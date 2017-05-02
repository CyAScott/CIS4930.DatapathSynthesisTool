﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Synthesize.Allocation;
using Synthesize.Extensions;
using Synthesize.FileParsing;

namespace Synthesize.DataPath
{
    public partial class DataPathGenerator
    {
        private string writeControllerValueFor(StreamWriter stream, RegisterUnit unit, int cycle, int index)
        {
            stream.Write($"\t\t\t\tcontrol_bus({index}) <= '");

            var inputReg = unit.Registers.OfType<InputRegister>().FirstOrDefault();

            if (cycle == 0 && inputReg != null)
            {
                stream.WriteLine($"1'; -- {unit.Name}: store input {inputReg.Name}");
                return "1";
            }

            var operation = AifFile.Operations.Values.FirstOrDefault(op => op.CycleIndex == cycle && unit.Registers.Any(reg => reg == op.Output));

            stream.WriteLine(operation != null ?
                $"1'; -- {unit.Name}: store output from operation {operation.Name}" :
                $"0'; -- {unit.Name}: keep value");

            return operation != null ? "1" : "0";
        }
        private string writeControllerValueFor(StreamWriter stream, Multiplexer.Multiplexer multiplexer, int cycle, int index)
        {
            stream.Write("\t\t\t\tcontrol_bus(");

            var value = multiplexer.GetSelectValueAndNameFrom(cycle);
            
            stream.Write(value.Item1.Length == 1 ?
                $"{index}) <= '{value.Item1}'" :
                $"{index} to {index + value.Item1.Length - 1}) <= \"{value.Item1}\"");
            stream.Write($"; -- {value.Item1} {multiplexer.Name}");

            if (string.IsNullOrEmpty(value.Item2))
            {
                stream.WriteLine();
            }
            else
            {
                stream.WriteLine($": select {value.Item2}");
            }

            return value.Item1;
        }
        private void writeControllerComments(StreamWriter stream)
        {
            stream.WriteTextResource("CommentLine");
            stream.WriteLine("--");

            stream.WriteLine("-- Functional Unit Multiplexers:");
            stream.WriteLine("--");
            if (Multiplexers.FunctionalUnitMultiplexers.Length == 0)
            {
                stream.WriteLine("--\tNone");
            }
            else
            {
                foreach (var multiplexer in Multiplexers.FunctionalUnitMultiplexers)
                {
                    stream.WriteLine($"--\t{multiplexer}");
                }
            }
            stream.WriteLine("--");

            stream.WriteLine("-- Register Multiplexers:");
            stream.WriteLine("--");
            if (Multiplexers.FunctionalUnitMultiplexers.Length == 0)
            {
                stream.WriteLine("-- \tNone");
            }
            else
            {
                foreach (var multiplexer in Multiplexers.RegisterUnitMultiplexers)
                {
                    stream.WriteLine($"-- \t{multiplexer}");
                }
            }
            stream.WriteLine("--");

            stream.WriteLine("-- Order of Operations:");
            stream.WriteLine("--");
            for (var index = 0; index < Scheduler.Cycles.Length; index++)
            {
                stream.WriteLine($"-- Cycle {index}: " + string.Join(", ", Scheduler.Cycles[index].Select(op => $"{op.Op}.{op.Name}({op.Left.Name}, {op.Right.Name})")));
            }
            stream.WriteLine("--");

            stream.WriteLine("-- Expressions:");
            stream.WriteLine("--");
            foreach (var expression in AifFile.AsExpressions)
            {
                stream.WriteLine("--\t" + expression);
            }
            stream.WriteLine("--");

            stream.WriteTextResource("CommentLine");
        }
        private void writeControllerEntity(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("entity input_controller is");
            stream.WriteLine("\tport");
            stream.WriteLine("\t(");
            stream.WriteLine("\t\tclock, reset, s_tart : IN std_logic;");
            stream.WriteLine("\t\tfinish : OUT std_logic;");
            stream.WriteLine($"\t\tcontrol_out : OUT std_logic_vector(0 to {ControllerBusBitWidth - 1})");
            stream.WriteLine("\t);");
            stream.WriteLine("end input_controller;");
        }
        private void writeControllerState(StreamWriter stream, int cycle, int stateCount)
        {
            stream.WriteLine($"\t\t\twhen {cycle} =>");

            var index = 0;
            var busValue = new StringBuilder();

            //set values for control wires to registers to tell them when the store the current input
            foreach (var unit in RegisterAllocator.Units)
            {
                busValue.Append(writeControllerValueFor(stream, unit, cycle, index));
                index++;
            }

            //set the select values for each multiplexer
            foreach (var multiplexer in Multiplexers.Multiplexers)
            {
                var value = writeControllerValueFor(stream, multiplexer, cycle, index);
                busValue.Append(value);
                index += value.Length;
            }

            writeControllerValue(stream, busValue.ToString());

            if (cycle + 1 < stateCount)
            {
                stream.WriteLine("\t\t\t\tinternal_finish <= '0';");
                stream.WriteLine($"\t\t\t\tnext_state <= {cycle + 1};");
            }
            else
            {
                stream.WriteLine("\t\t\t\tinternal_finish <= '1';");
                stream.WriteLine("\t\t\t\tnext_state <= 0;");
            }
        }
        private void writeControllerValue(StreamWriter stream, string value)
        {
            stream.WriteLine($"\t\t\t\t-- Binary: {value}");

            var hexDigits = new List<char>();
            for (var index = value.Length - 1; index >= 0; index -= 4)
            {
                var fourBits = value.Substring(Math.Max(0, index - 3), Math.Min(4, value.Length - index + 3)).PadLeft(4, '0').Replace('X', '0');

                hexDigits.Add(Convert.ToByte(fourBits, 2).ToString("x1")[0]);
            }
            hexDigits.Reverse();

            stream.WriteLine($"\t\t\t\t-- Hex: {new string(hexDigits.ToArray())}");
        }
        
        protected int ControllerBusBitWidth => Multiplexers.BusBitWidth + RegisterAllocator.Units.Length;
        protected int GetControlIndex(Multiplexer.Multiplexer multiplexer)
        {
            return RegisterAllocator.Units.Length + Multiplexers.Multiplexers
                .TakeWhile(item => item != multiplexer)
                .Sum(item => item.SelectionBitSize);
        }

        /// <summary>
        /// Generates the VHDL file that controls all the multiplexers.
        /// </summary>
        public void SaveController(StreamWriter stream)
        {
            writeControllerComments(stream);

            stream.WriteLine();
            stream.WriteLine("library IEEE;");
            stream.WriteLine("use IEEE.std_logic_1164.all;");

            writeControllerEntity(stream);

            var stateCount = Multiplexers.StateCount;
            stream.WriteLine();
            stream.WriteLine("architecture moore of input_controller is");
            stream.WriteLine();
            stream.WriteLine("\tsignal current_state, next_state : integer := 0;");
            stream.WriteLine("\tsignal internal_finish : std_logic := '0';");
            stream.WriteLine($"\tsignal control_bus : std_logic_vector(0 to {ControllerBusBitWidth - 1}) := \"{new string('0', ControllerBusBitWidth)}\";");
            stream.WriteLine();
            stream.WriteLine("begin");


            stream.WriteLine();
            stream.WriteLine("\tprocess(current_state)");
            stream.WriteLine("\tbegin");
            stream.WriteLine("\t\tcase current_state is");
            foreach (var cycle in Enumerable.Range(0, stateCount))
            {
                writeControllerState(stream, cycle, stateCount);
            }
            stream.WriteLine("\t\t\twhen others => null;");
            stream.WriteLine("\t\tend case;");
            stream.WriteLine("\tend process;");


            stream.WriteLine();
            stream.WriteLine("\tprocess(clock, reset)");
            stream.WriteLine("\tbegin");
            stream.WriteLine("\t\tif (reset = '1' and reset'event) then");
            stream.WriteLine("\t\t\tcurrent_state <= 0;");
            stream.WriteLine("\t\t\tcontrol_out <= control_bus;");
            stream.WriteLine("\t\t\tfinish <= internal_finish;");
            stream.WriteLine("\t\telsif (clock = '1' and clock'event) then");
            stream.WriteLine("\t\t\tcurrent_state <= next_state;");
            stream.WriteLine("\t\t\tcontrol_out <= control_bus;");
            stream.WriteLine("\t\t\tfinish <= internal_finish;");
            stream.WriteLine("\t\tend if;");
            stream.WriteLine("\tend process;");

            stream.WriteLine();
            stream.WriteLine("end moore;");
        }
    }
}
