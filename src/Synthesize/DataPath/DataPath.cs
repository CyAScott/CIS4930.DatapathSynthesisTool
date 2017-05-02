using System;
using System.IO;
using System.Linq;
using Synthesize.Allocation;
using Synthesize.Extensions;
using Synthesize.FileParsing;

namespace Synthesize.DataPath
{
    public partial class DataPathGenerator
    {
        private int writeDataPathMultiplexerRegister(StreamWriter stream, int inputBitIndex, RegisterBase register)
        {
            var regUnit = RegisterAllocator.Units.FirstOrDefault(registerUnit => registerUnit.Registers.Contains(register));
            if (regUnit != null)
            {
                //must be a shared register unit
                stream.WriteLine($"\t\t\tinput({inputBitIndex + regUnit.Bits - 1} downto {inputBitIndex}) => {regUnit.Name}_out({regUnit.Bits - 1} downto 0), -- {register.Name}");
                return regUnit.Bits;
            }

            //must be an input
            stream.WriteLine($"\t\t\tinput({inputBitIndex + register.Bits - 1} downto {inputBitIndex}) => {register.Name}_out({register.Bits - 1} downto 0), -- {register.Name}");
            return register.Bits;
        }
        private void writeDataPathComments(StreamWriter stream)
        {
            stream.WriteTextResource("CommentLine");
            stream.WriteLine("--");
            stream.WriteLine("-- Registers:");
            foreach (var reg in RegisterAllocator.Units.Select(unit => $"{unit.Name} ({string.Join(", ", unit.Registers.Select(reg => reg.Name))})"))
            {
                stream.WriteLine("--\t" + reg);
            }
            stream.WriteLine("-- Functional Units:");
            foreach (var unit in Functional.Units.Select(unit => $"{unit.Name} ({string.Join(", ", unit.Operations.Select(op => op.Name))})"))
            {
                stream.WriteLine("--\t" + unit);
            }
            stream.WriteLine("-- Multiplexers:");
            foreach (var multiplexer in Multiplexers.Multiplexers.Select(unit => $"{unit.Name} ({string.Join(", ", unit.ItemNames)})"))
            {
                stream.WriteLine("--\t" + multiplexer);
            }
            stream.WriteLine("-- Expressions:");
            foreach (var expression in AifFile.AsExpressions)
            {
                stream.WriteLine("--\t" + expression);
            }
            stream.WriteLine("--");
            stream.WriteTextResource("CommentLine");
        }
        private void writeDataPathComponents(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteTextResource("c_multiplexer", "\t");
            stream.WriteLine("\tfor all : c_multiplexer use entity work.c_multiplexer(behavior);");

            stream.WriteLine();
            stream.WriteTextResource("c_register", "\t");
            stream.WriteLine("\tfor all : c_register use entity work.c_register(behavior);");

            foreach (var unit in Functional.Units
                .Select(unit => unit.VhdlCodeFile)
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                stream.WriteLine();
                stream.WriteTextResource(unit, "\t");
                stream.WriteLine($"\tfor all : {unit} use entity work.{unit}(behavior);");
            }
        }
        private void writeDataPathEntity(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("entity input_dp is");
            stream.WriteLine("\tport");
            stream.WriteLine("\t(");
            var registers = AifFile.Registers.Values.Where(reg => reg is InputRegister || reg is OutputRegister).ToArray();
            foreach (var group in registers
                .GroupBy(item => $"{(item is InputRegister ? "IN" : "OUT")} std_logic_vector({item.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\t\t{string.Join(", ", group.Select(reg => reg.Name))} : {group.Key};");
            }
            stream.WriteLine($"\t\tctrl : IN std_logic_vector(0 to {ControllerBusBitWidth - 1});");
            stream.WriteLine("\t\tclear, clock : IN std_logic");
            stream.WriteLine("\t);");
            stream.WriteLine("end input_dp;");
        }
        private void writeDataPathFunctionalUnit(StreamWriter stream, FunctionalUnit functionalUnit)
        {
            stream.WriteLine();
            stream.WriteLine($"\t-- {functionalUnit.Name} {functionalUnit.Op}({string.Join(", ", functionalUnit.Operations.Select(reg => reg.Name))})");
            stream.WriteLine($"\t{functionalUnit.Name} : {functionalUnit.VhdlCodeFile}");
            stream.WriteLine($"\t\tgeneric map({functionalUnit.Bits})");
            stream.WriteLine("\t\tport map");
            stream.WriteLine("\t\t(");

            //wire inputs
            var multiplexerInput = Multiplexers.FunctionalUnitMultiplexers.FirstOrDefault(multiplexer => multiplexer.Unit == functionalUnit);
            if (multiplexerInput != null)
            {
                var bits = multiplexerInput.OutputBitSize / 2;
                
                stream.WriteLine($"\t\t\tinput1({bits - 1} downto 0) => {multiplexerInput.Name}_out({bits - 1} downto 0), -- {string.Join(", ", multiplexerInput.Op.Select(op => op.Left.Name))}");
                stream.WriteLine($"\t\t\tinput2({bits - 1} downto 0) => {multiplexerInput.Name}_out({multiplexerInput.OutputBitSize - 1} downto {bits}), -- {string.Join(", ", multiplexerInput.Op.Select(op => op.Right.Name))}");
            }
            else
            {
                //wire up the input 1 (left)
                var left = RegisterAllocator.Units.First(registerUnit =>
                    functionalUnit.Operations.All(op => registerUnit.Registers.Contains(op.Left)));
                stream.WriteLine($"\t\t\tinput1({functionalUnit.Bits - 1} downto 0) => {left.Name}_out({left.Bits - 1} downto 0), -- {string.Join(",", functionalUnit.Operations.Select(op => op.Left.Name))}");

                //wire up the input 2 (right)
                var right = RegisterAllocator.Units.First(registerUnit =>
                    functionalUnit.Operations.All(op => registerUnit.Registers.Contains(op.Right)));
                stream.WriteLine($"\t\t\tinput2({functionalUnit.Bits - 1} downto 0) => {right.Name}_out({right.Bits - 1} downto 0), -- {string.Join(",", functionalUnit.Operations.Select(op => op.Right.Name))}");
            }

            //wire up the output
            stream.WriteLine($"\t\t\toutput({functionalUnit.Bits - 1} downto 0) => {functionalUnit.Name}_out({functionalUnit.Bits - 1} downto 0)");

            stream.WriteLine("\t\t);");
        }
        private void writeDataPathMultiplexer(StreamWriter stream, Multiplexer.Multiplexer multiplexer)
        {
            stream.WriteLine();
            stream.WriteLine($"\t-- {multiplexer.Name}: {string.Join(", ", multiplexer.ItemNames)}");
            stream.WriteLine($"\t{multiplexer.Name} : c_multiplexer");
            stream.WriteLine($"\t\tgeneric map({multiplexer.OutputBitSize}, {multiplexer.ItemCount}, {multiplexer.SelectionBitSize})");
            stream.WriteLine("\t\tport map");
            stream.WriteLine("\t\t(");

            //wire up inputs
            var inputBitIndex = 0;
            foreach (var item in multiplexer.Items)
            {
                var register = item as RegisterBase;
                if (register != null)
                {
                    if (register is InputRegister)
                    {
                        stream.WriteLine($"\t\t\tinput({inputBitIndex + register.Bits - 1} downto {inputBitIndex}) => {register.Name}({register.Bits - 1} downto 0), -- {register.Name}");
                        inputBitIndex += register.Bits;
                    }
                    else
                    {
                        var operation = AifFile.Operations.Values.First(op => op.Output == register);
                        var operationUnit = Functional.Units.First(unit => unit.Operations.Contains(operation));
                        
                        stream.WriteLine($"\t\t\tinput({inputBitIndex + register.Bits - 1} downto {inputBitIndex}) => {operationUnit.Name}_out({register.Bits - 1} downto 0), -- {register.Name}");
                        inputBitIndex += register.Bits;
                    }
                }
                else
                {
                    var op = (Operation)item;
                    stream.WriteLine($"\t\t\t-- Operation {op.Name}: {op.Op}({op.Left.Name}, {op.Right.Name})");
                    inputBitIndex += writeDataPathMultiplexerRegister(stream, inputBitIndex, op.Left);
                    inputBitIndex += writeDataPathMultiplexerRegister(stream, inputBitIndex, op.Right);
                }
            }

            //wire up selector
            var controlIndex = GetControlIndex(multiplexer);
            stream.Write("\t\t\tmux_select");
            stream.WriteLine(multiplexer.SelectionBitSize == 1 ?
                $"(0) => ctrl({controlIndex})," :
                $"({multiplexer.SelectionBitSize - 1} downto 0) => ctrl({controlIndex} to {controlIndex + multiplexer.SelectionBitSize - 1}),");

            //wire up the output
            stream.WriteLine($"\t\t\toutput => {multiplexer.Name}_out");

            stream.WriteLine("\t\t);");
        }
        private void writeDataPathOutputs(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("\t-- Primary outputs");
            foreach (var item in RegisterAllocator.Units
                .Select(unit => new
                {
                    name = unit.Name,
                    op = unit.Registers.OfType<OutputRegister>().FirstOrDefault()
                })
                .Where(item => item.op != null))
            {
                stream.WriteLine($"\t{item.op.Name}({item.op.Bits - 1} downto 0) <= {item.name}_out({item.op.Bits - 1} downto 0);");
            }
        }
        private void writeDataPathRegister(StreamWriter stream, RegisterUnit registerUnit)
        {
            var bits = registerUnit.Registers.Max(reg => reg.Bits);

            stream.WriteLine();
            stream.WriteLine($"\t-- {registerUnit.Name} ({string.Join(", ", registerUnit.Registers.Select(reg => reg.Name))})");
            stream.WriteLine($"\t{registerUnit.Name} : c_register");
            stream.WriteLine($"\t\tgeneric map({bits})");
            stream.WriteLine("\t\tport map");
            stream.WriteLine("\t\t(");
            stream.Write($"\t\t\tinput({bits - 1} downto 0) => ");

            //wire up the input
            if (registerUnit.Registers.Length == 1)
            {
                var reg = registerUnit.Registers.First();
                //if the input is an input variable
                if (reg is InputRegister)
                {
                    stream.WriteLine($"{reg.Name}({reg.Bits - 1} downto 0),");
                }
                //else if input is a single functional unit
                else
                {
                    var functionalUnit = Functional.Units.First(item => item.Operations.Any(op => op.Output == reg));
                    stream.WriteLine($"{functionalUnit.Name}_out({functionalUnit.Bits - 1} downto 0), -- Operations: {string.Join(", ", functionalUnit.Operations.Where(op => op.Output == reg).Select(op => op.Name))}");
                }
            }
            //else if the input is multiple function units
            else
            {
                var inputMultiplexor = Multiplexers.RegisterUnitMultiplexers.First(multiplexer => multiplexer.Unit == registerUnit);
                stream.WriteLine($"{inputMultiplexor.Name}_out({inputMultiplexor.OutputBitSize - 1} downto 0), -- Items: {string.Join(", ", inputMultiplexor.ItemNames)}");
            }

            //wire up the store value control wire
            stream.WriteLine($"\t\t\twr => ctrl({registerUnit.Index}),");

            //wire up the clear value control wire
            stream.WriteLine("\t\t\tclear => clear,");

            //wire up the clock signal
            stream.WriteLine("\t\t\tclock => clock,");

            //wire up the output
            stream.WriteLine($"\t\t\toutput => {registerUnit.Name}_out");

            stream.WriteLine("\t\t);");
        }
        private void writeDataPathVhdlComponents(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteTextResource("CommentLine");
            stream.WriteLine("--");
            stream.WriteLine("-- Unit components");
            stream.WriteLine("--");
            stream.WriteTextResource("CommentLine");

            stream.WriteLine();
            stream.WriteVhdlFile("c_multiplexer");

            stream.WriteLine();
            stream.WriteVhdlFile("c_register");

            foreach (var codeFile in Functional.Units
                .Select(unit => unit.VhdlCodeFile)
                .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                stream.WriteLine();
                stream.WriteVhdlFile(codeFile);
            }
        }
        private void writeDataPathWires(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("\t-- Outputs of registers");
            foreach (var group in RegisterAllocator.Units
                .GroupBy(unit => $"std_logic_vector({unit.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\tsignal {string.Join(", ", group.Select(unit => unit.Name + "_out"))} : {group.Key};");
            }

            stream.WriteLine();
            stream.WriteLine("\t-- Outputs of FUs");
            foreach (var group in Functional.Units
                .GroupBy(unit => $"std_logic_vector({unit.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\tsignal {string.Join(", ", group.Select(unit => unit.Name + "_out"))} : {group.Key};");
            }

            stream.WriteLine();
            stream.WriteLine("\t-- Outputs of Interconnect Units");
            foreach (var group in Multiplexers.Multiplexers
                .GroupBy(multiplexer => $"std_logic_vector({multiplexer.OutputBitSize - 1} downto 0)"))
            {
                stream.WriteLine($"\tsignal {string.Join(", ", group.Select(multiplexer => multiplexer.Name + "_out"))} : {group.Key};");
            }
        }

        /// <summary>
        /// Generates the VHDL file links the inputs and outputs to the shared resources (registers and functional units).
        /// </summary>
        public void SaveDataPath(StreamWriter stream, bool signalFile = false)
        {
            if (signalFile)
            {
                writeDataPathVhdlComponents(stream);
            }

            writeDataPathComments(stream);

            stream.WriteLine();
            stream.WriteLine("library IEEE;");
            stream.WriteLine("use IEEE.std_logic_1164.all;");

            writeDataPathEntity(stream);

            stream.WriteLine();
            stream.WriteLine("architecture rtl of input_dp is");

            writeDataPathComponents(stream);

            writeDataPathWires(stream);

            stream.WriteLine();
            stream.WriteLine("begin");

            stream.WriteLine();
            stream.WriteLine("\t-- Registers");
            foreach (var registerUnit in RegisterAllocator.Units)
            {
                writeDataPathRegister(stream, registerUnit);
            }

            stream.WriteLine();
            stream.WriteLine("\t-- Functional Units");
            foreach (var functionalUnit in Functional.Units)
            {
                writeDataPathFunctionalUnit(stream, functionalUnit);
            }

            stream.WriteLine();
            stream.WriteLine("\t-- Multiplexers");
            foreach (var multiplexer in Multiplexers.Multiplexers)
            {
                writeDataPathMultiplexer(stream, multiplexer);
            }

            writeDataPathOutputs(stream);

            stream.WriteLine();
            stream.WriteLine("end rtl;");
        }
    }
}
