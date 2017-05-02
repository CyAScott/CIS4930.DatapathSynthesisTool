using System;
using System.Linq;
using Synthesize.CliquePartition;
using Synthesize.FileParsing;

namespace Synthesize.Allocation
{
    public class RegisterAllocator : AllocatorBase
    {
        private RegisterBase[] getRegisterLifeCycles()
        {
            var operations = Functional.Scheduler.AifFile.Operations.Values.ToArray();

            var registers = Functional.Scheduler.AifFile.Registers.Values.ToArray();

            //find the life span for each register
            foreach (var register in registers)
            {
                var operationsThatUseRegister = new Lazy<Operation[]>(() => operations
                    .Where(op => op.Left == register || op.Right == register || op.Output == register)
                    .ToArray());

                register.StartCycle = register is InputRegister ? 0 : operationsThatUseRegister.Value.Min(op => op.CycleIndex);
                register.StopCycle = register is OutputRegister ? Functional.Scheduler.Cycles.Length - 1 : operationsThatUseRegister.Value.Max(op => op.CycleIndex);

                Log.Info($"{register.Name, -10} -> ({register.StartCycle:00}, {register.StopCycle:00}) {new string('_', register.StartCycle)}{new string('*', register.StopCycle - register.StartCycle + 1)}{new string('_', Functional.Scheduler.Cycles.Length - register.StopCycle - 1)}");
            }

            return registers;
        }
        private static int[][] buildCompatibilityGraph(RegisterBase[] registers)
        {
            var compatibilityGraph = registers
                   .Select(reg1 => new int[Array.IndexOf(registers, reg1)].Concat(registers
                       .SkipWhile(reg2 => reg1 != reg2)
                       .Select(reg2 => reg1 == reg2 || reg1.IsCompatible(reg2) ? 1 : 0))
                       .ToArray())
                   .ToArray();

            for (var row = 1; row < compatibilityGraph.Length; row++)
            {
                for (var col = 0; col < row; col++)
                {
                    compatibilityGraph[row][col] = compatibilityGraph[col][row];
                }
            }

            return compatibilityGraph;
        }

        public RegisterAllocator(FunctionalUnitAllocator functional)
        {
            Functional = functional;
            
            var registers = getRegisterLifeCycles();

            var compatibilityGraph = buildCompatibilityGraph(registers);
            
            var cliques = CliqueHelper.CliquePartition(compatibilityGraph);

            Units = cliques.Select((clique, cliqueIndex) => new RegisterUnit
            {
                Index = cliqueIndex,
                Registers = clique.Members.Take(clique.Size)
                    .Select(index => registers[index])
                    .OrderBy(reg => reg.Name)
                    .ToArray()
            })
            .ToArray();
            
            foreach (var unit in Units)
            {
                Log.Info(unit);
            }
        }
        public RegisterUnit[] Units { get; }
        public FunctionalUnitAllocator Functional { get; }
    }
}
