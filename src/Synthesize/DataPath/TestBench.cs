using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Synthesize.Extensions;
using Synthesize.FileParsing;

namespace Synthesize.DataPath
{
    public partial class DataPathGenerator
    {
        private RegisterBase[] writeTestBenchComponents(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("\tcomponent input");
            stream.WriteLine("\t\tport");
            stream.WriteLine("\t\t(");
            var registers = AifFile.Registers.Values.Where(reg => reg is InputRegister || reg is OutputRegister).ToArray();
            foreach (var group in registers
                .GroupBy(item => $"{(item is InputRegister ? "IN" : "OUT")} std_logic_vector({item.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\t\t\t{string.Join(", ", group.Select(reg => reg.Name))} : {group.Key};");
            }
            stream.WriteLine("\t\t\tclear, clock, s_tart : IN std_logic;");
            stream.WriteLine("\t\t\tfinish : OUT std_logic");
            stream.WriteLine("\t\t);");
            stream.WriteLine("\tend component;");
            stream.WriteLine("\tfor all : input use entity work.input(rtl);");

            return registers;
        }
        private void writeTestBenchComments(StreamWriter stream)
        {
            stream.WriteTextResource("CommentLine");
            stream.WriteLine("--");
            stream.WriteLine("-- Inputs: " + string.Join(", ", AifFile.Registers.Values.OfType<InputRegister>().Select(reg => reg.Name)));
            stream.WriteLine("-- Output(s): " + string.Join(", ", AifFile.Registers.Values.OfType<OutputRegister>().Select(reg => reg.Name)));
            stream.WriteLine("-- Expressions:");
            foreach (var expression in AifFile.AsExpressions)
            {
                stream.WriteLine("--\t" + expression);
            }
            stream.WriteLine("--");
            stream.WriteTextResource("CommentLine");
        }
        private void writeTestBenchTestUnit(StreamWriter stream, RegisterBase[] registers)
        {
            stream.WriteLine();
            stream.WriteLine($"\ttest_input : input port map ({string.Join(", ", registers.Select(reg => reg.Name))}, clear, clock, s_tart, finish);");
        }
        private void writeTestBenchWires(StreamWriter stream, RegisterBase[] registers)
        {
            stream.WriteLine();
            foreach (var group in registers
                .GroupBy(item => $"std_logic_vector({item.Bits - 1} downto 0) := \"{new string('0', item.Bits)}\""))
            {
                stream.WriteLine($"\tsignal {string.Join(", ", group.Select(reg => reg.Name))} : {group.Key};");
            }
            stream.WriteLine("\tsignal clear, clock, finish, s_tart : std_logic := '0';");
        }

        public void SaveTestBench(StreamWriter stream, Dictionary<string, string>[] inputAndOutputs)
        {
            writeTestBenchComments(stream);

            stream.WriteLine();
            stream.WriteLine("library IEEE;");
            stream.WriteLine("use IEEE.std_logic_1164.all;");

            stream.WriteLine();
            stream.WriteLine("entity test_me_tb is");
            stream.WriteLine("end test_me_tb;");

            stream.WriteLine();
            stream.WriteLine("architecture test of test_me_tb is");

            var registers = writeTestBenchComponents(stream);
            writeTestBenchWires(stream, registers);

            stream.WriteLine();
            stream.WriteLine("begin");

            writeTestBenchTestUnit(stream, registers);

            stream.WriteLine();
            stream.WriteLine("\tprocess");
            stream.WriteLine("\t\tbegin");
            stream.WriteLine("\t\t\twait for 1 ns;");

            foreach (var testCase in inputAndOutputs)
            {
                var values = testCase.ToDictionary(pair => pair.Key, pair => Convert.ToInt64(pair.Value, 2));

                stream.WriteLine();
                foreach (var expression in AifFile.AsExpressions)
                {
                    stream.WriteLine("\t\t\t-- " + expression);
                }
                foreach (var reg in registers.OfType<InputRegister>())
                {
                    stream.WriteLine($"\t\t\t{reg.Name} <= \"{testCase[reg.Name]}\"; -- {values[reg.Name]}");
                }
                stream.WriteLine("\t\t\ts_tart <= '1';");
                stream.WriteLine("\t\t\tclock <= '1'; wait for 1 ns;");
                stream.WriteLine("\t\t\tclock <= '0'; wait for 1 ns;");
                for (var cycle = 1; cycle < Scheduler.Cycles.Length; cycle++)
                {
                    stream.WriteLine("\t\t\tclock <= '1'; wait for 1 ns;");
                    stream.WriteLine("\t\t\tclock <= '0'; wait for 1 ns;");
                }
                foreach (var output in registers.OfType<OutputRegister>())
                {
                    stream.WriteLine($"\t\t\tassert {output.Name} = \"{testCase[output.Name]}\" report \"{AifFile.GetExpression(output, values)}\" severity failure;");
                }
            }

            stream.WriteLine();
            stream.WriteLine("\t\t\twait;");
            stream.WriteLine("\tend process;");
            stream.WriteLine("end test;");
        }
    }
}
