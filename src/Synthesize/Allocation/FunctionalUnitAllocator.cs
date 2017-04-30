using System;
using System.Linq;
using Synthesize.CliquePartition;
using Synthesize.Scheduler;

namespace Synthesize.Allocation
{
    public class FunctionalUnitAllocator : AllocatorBase
    {
        public FunctionalUnitAllocator(SchedulerBase scheduler)
        {
            Scheduler = scheduler;

            Units = scheduler.AifFile.Operations.Values
                .GroupBy(op => op.Op)
                .Select(group => new
                {
                    Op = group.Key,
                    Operations = group.OrderBy(op => op.Id).ToArray()
                })
                .Select(operationInfo => new
                {
                    OperationInfo = operationInfo,
                    CompatibilityGraph = operationInfo.Operations
                        .Select(op1 => new int[Array.IndexOf(operationInfo.Operations, op1)].Concat(operationInfo.Operations
                            .SkipWhile(op2 => op1 != op2)
                            .Select(op2 =>
                                //if both ops are the same or
                                op1 == op2 ||
                                //if both ops have different cycle indices and
                                (op1.CycleIndex != op2.CycleIndex &&
                                 //ops are not dependant on each other
                                 !op1.IsDependantOn(op2)) ? 1 : 0))
                            .ToArray())
                        .ToArray()
                })
                .Select(graph =>
                {
                    for (var row = 1; row < graph.CompatibilityGraph.Length; row++)
                    {
                        for (var col = 0; col < row; col++)
                        {
                            graph.CompatibilityGraph[row][col] = graph.CompatibilityGraph[col][row];
                        }
                    }
                    return graph;
                })
                .Select(graph => new
                {
                    Cliques = CliqueHelper.CliquePartition(graph.CompatibilityGraph),
                    Graph = graph
                })
                .SelectMany(unitGroup => unitGroup
                    .Cliques.Select((clique, cliqueIndex) => new FunctionalUnit
                    {
                        Index = cliqueIndex,
                        Op = unitGroup.Graph.OperationInfo.Op,
                        Operations = clique.Members.Take(clique.Size)
                            .Select(index => unitGroup.Graph.OperationInfo.Operations[index])
                            .OrderBy(op => op.Id)
                            .ToArray(),
                        VhdlCodeFile = unitGroup.Graph.OperationInfo.Operations.First().VhdlCodeFile
                    }))
                .ToArray();

            foreach (var unit in Units)
            {
                Log.Info(unit);
            }
        }
        public FunctionalUnit[] Units { get; }
        public SchedulerBase Scheduler { get; }
    }
}
