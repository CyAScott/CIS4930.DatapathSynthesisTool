using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SolverFoundation.Services;
using Synthesize.FileParsing;

namespace Synthesize.Scheduler
{
    public class IlpScheduler : SchedulerBase
    {
        public class OperationCycleRange
        {
            private Term getDependantSequencingTerm()
            {
                Term metaTerm = null;
                for (var cycle = MinCycle; cycle <= MaxCycle; cycle++)
                {
                    metaTerm = cycle == MinCycle ? cycle * Variables[cycle] : metaTerm + cycle * Variables[cycle];
                }
                return metaTerm;
            }

            public OperationCycleRange(IlpScheduler parent)
            {
                Parent = parent;
            }
            public Dictionary<int, Decision> Variables { get; } = new Dictionary<int, Decision>();
            public IEnumerable<Term> GetSequencingTerms()
            {
                //Sequencing relations must be satisfied

                Term thisSequencingTerm = null;
                for (var cycle = MinCycle; cycle <= MaxCycle; cycle++)
                {
                    thisSequencingTerm = cycle == MinCycle ? -cycle * Variables[cycle] : thisSequencingTerm - cycle * Variables[cycle];
                }

                var leftOp = Parent.OperationCycleRanges.FirstOrDefault(item => item.Operation.Output == Operation.Left);
                if (leftOp != null)
                {
                    yield return leftOp.getDependantSequencingTerm() + thisSequencingTerm + 1 <= 0;
                }

                var rightOp = Parent.OperationCycleRanges.FirstOrDefault(item => item.Operation.Output == Operation.Right);
                if (rightOp != null)
                {
                    yield return rightOp.getDependantSequencingTerm() + thisSequencingTerm + 1 <= 0;
                }
            }
            public IlpScheduler Parent { get; }
            public Operation Operation { get; set; }
            public Term CreateVariables()
            {
                //add a variable to the linear system for each possible cycle interval this operation can be placed
                Term startOnlyOnceTerm = null;
                for (var cycle = MinCycle; cycle <= MaxCycle; cycle++)
                {
                    var decision = new Decision(Domain.IntegerNonnegative, $"{Operation.Id}_c{cycle:000}");

                    Parent.ProblemModel.AddDecision(Variables[cycle] = decision);

                    startOnlyOnceTerm = cycle == MinCycle ? decision : startOnlyOnceTerm + decision;
                }

                Parent.Log.Trace($"Operation ({Operation.Name} = {Operation.Id}) Variables: {string.Join(", ", Variables.Values.Select(item => item.Name))}");

                //return a constraint that makes it only possible for this operation to be assign to only one cycle interval
                return startOnlyOnceTerm == 1;
            }
            public int MaxCycle { get; set; } = -1;
            public int MinCycle { get; set; } = -1;
            public override string ToString()
            {
                return $"From {MinCycle:000} to {MaxCycle:000} -> {Operation.Id} - {Operation}";
            }
        }

        private Dictionary<string, Decision> resources;
        private IEnumerable<Term> getResourceConstraints()
        {
            foreach (var cycle in Enumerable.Range(1, AifFile.MinCycles.Values.Max() + 1))
            {
                foreach (var group in OperationCycleRanges
                    .Where(op => op.Variables.ContainsKey(cycle))
                    .GroupBy(op => op.Operation.Op))
                {
                    var firstTerm = true;
                    Term resourceConstraint = null;
                    foreach (var op in group)
                    {
                        var variable = op.Variables[cycle];
                        resourceConstraint = firstTerm ? variable : resourceConstraint + variable;
                        firstTerm = false;
                    }
                    yield return resourceConstraint <= resources[group.Key];
                }
            }
        }
        private OperationCycleRange finMaxCycle(Operation op, List<OperationCycleRange> operationCycleRanges, int latency)
        {
            var cachedSlack = operationCycleRanges.FirstOrDefault(item => item.Operation == op);
            if (cachedSlack != null && cachedSlack.MaxCycle > -1)
            {
                return cachedSlack;
            }

            var returnValue = cachedSlack ?? new OperationCycleRange(this)
            {
                Operation = op
            };
            
            returnValue.MaxCycle = op.Output is OutputRegister ?

                    Math.Max(AifFile.MinCycles[op.Output.Name], latency):

                    AifFile.Operations.Values
                        .Where(item => item.Left == op.Output || item.Right == op.Output)
                        .Min(item => finMaxCycle(item, operationCycleRanges, latency).MaxCycle - 1);

            if (cachedSlack == null)
            {
                operationCycleRanges.Add(returnValue);
            }

            return returnValue;
        }
        private OperationCycleRange findMinCycle(Operation op, List<OperationCycleRange> operationCycleRanges)
        {
            var cachedSlack = operationCycleRanges.FirstOrDefault(item => item.Operation == op);
            if (cachedSlack != null && cachedSlack.MinCycle > -1)
            {
                return cachedSlack;
            }

            var returnValue = cachedSlack ?? new OperationCycleRange(this)
            {
                Operation = op
            };

            var leftReg = op.Left as RegisterWithParentBase;
            var rightReg = op.Right as RegisterWithParentBase;

            returnValue.MinCycle = Math.Max(
                leftReg == null ? 1 : findMinCycle(leftReg.Parent, operationCycleRanges).MinCycle + 1, 
                rightReg == null ? 1 : findMinCycle(rightReg.Parent, operationCycleRanges).MinCycle + 1);

            if (cachedSlack == null)
            {
                operationCycleRanges.Add(returnValue);
            }

            return returnValue;
        }
        private void addResources()
        {
            //create a variable for each resource
            var firstTerm = true;
            Term resourceTerm = null;
            resources = new Dictionary<string, Decision>();
            foreach (var op in AifFile.OperationTypes)
            {
                var decision = new Decision(Domain.IntegerNonnegative, op);
                ProblemModel.AddDecision(decision);
                resources[op] = decision;

                resourceTerm = firstTerm ? decision : resourceTerm + decision;
                firstTerm = false;
            }

            Log.Debug("Resource Types: " + string.Join(", ", AifFile.OperationTypes));

            Log.Trace("Constraint (ResourceLowerBounds): " + ProblemModel.AddConstraints("ResourceLowerBounds", resources.Values
                .Select(decision => decision >= 1)
                .ToArray()).Expression);

            Log.Trace($"Resources Goal: {ProblemModel.AddGoal("resources", GoalKind.Minimize, resourceTerm).Expression}");
        }
        private void buildLinearModel()
        {
            addResources();

            //add a constraint that ensures that an operation only starts once
            Log.Trace("Constraint (OperationsStartOnlyOnce): " + ProblemModel.AddConstraints("OperationsStartOnlyOnce", OperationCycleRanges
                .Select(op => op.CreateVariables())
                .ToArray()).Expression);
            
            //add a constraint that ensures sequencing relations are satisfied
            Log.Trace("Constraint (SequencingRelations): " + ProblemModel.AddConstraints("SequencingRelations", OperationCycleRanges
                .SelectMany(op => op.GetSequencingTerms())
                .ToArray()).Expression);

            //Resource bounds must be satisfied
            Log.Trace($"Constraint (ResourceBounds): {ProblemModel.AddConstraints("ResourceBounds", getResourceConstraints().ToArray()).Expression}");
        }
        private void buildOperationCycleRanges(int latency)
        {
            Log.Info("Finding the slack for each operation.");
            var operationCycleRanges = new List<OperationCycleRange>();
            foreach (var op in AifFile.Operations.Values.OrderBy(op => op.Id))
            {
                finMaxCycle(op, operationCycleRanges, latency);

                findMinCycle(op, operationCycleRanges);
            }
            OperationCycleRanges = operationCycleRanges.ToArray();

            foreach (var op in operationCycleRanges.OrderBy(op => op.Operation.Id))
            {
                Log.Debug(op);
            }
        }
        
        public IlpScheduler(AifFile file, int latency = -1)
            : base(file)
        {
            Log.Info("Generating schedule with integer linear programming.");

            buildOperationCycleRanges(latency);

            Log.Info("Setting up model for ILP.");
            Context = SolverContext.GetContext();
            Context.ClearModel();
            ProblemModel = Context.CreateModel();

            buildLinearModel();

            var solution = Context.Solve();
            if (solution.Quality != SolverQuality.Optimal)
            {
                throw new ArgumentException("Unable to find an optimal solution.");
            }

            var cycleCount = OperationCycleRanges.Max(item => item.MaxCycle) + 1;
            foreach (var op in OperationCycleRanges.OrderBy(op => op.Operation.Id))
            {
                Log.Info($"{op.Operation.Name} = " + string.Join(", ", Enumerable.Range(1, cycleCount)
                    .Select(cycle => op.Variables.ContainsKey(cycle) ? op.Variables[cycle].ToDouble() : 0)));
            }

            var report = solution.GetReport();
            Log.Trace(report.ToString());
        }
        public Model ProblemModel { get; }
        public OperationCycleRange[] OperationCycleRanges { get; private set; }
        public SolverContext Context { get; }
        public override void BuildSchedule()
        {
            Cycles = Enumerable.Range(1, AifFile.Operations.Count + 1)
                .Select(cycle => OperationCycleRanges
                    .Where(op => 
                        op.Variables.ContainsKey(cycle) &&
                        op.Variables[cycle].ToDouble() > 0)
                    .Select(op => op.Operation)
                    .ToArray())
                .Where(item => item.Length > 0)
                .ToArray();

            Resources = resources.ToDictionary(pair => pair.Key, pair => Convert.ToInt32(pair.Value.ToDouble()));
            
            PrintSchedule();
        }
    }
}
