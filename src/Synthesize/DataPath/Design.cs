using System.IO;
using System.Linq;
using Synthesize.Extensions;
using Synthesize.FileParsing;

namespace Synthesize.DataPath
{
    public partial class DataPathGenerator
    {
        private RegisterBase[] writeDesignEntity(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("entity input is");
            stream.WriteLine("\tport");
            stream.WriteLine("\t(");
            var registers = AifFile.Registers.Values.Where(reg => reg is InputRegister || reg is OutputRegister).ToArray();
            foreach (var group in registers
                .GroupBy(item => $"{(item is InputRegister ? "IN" : "OUT")} std_logic_vector({item.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\t\t{string.Join(", ", group.Select(reg => reg.Name))} : {group.Key};");
            }
            stream.WriteLine("\t\tclear, clock, s_tart : IN std_logic;");
            stream.WriteLine("\t\tfinish : OUT std_logic");
            stream.WriteLine("\t);");
            stream.WriteLine("end input;");

            return registers;
        }
        private void writeDesignComments(StreamWriter stream)
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
        private void writeDesignController(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("\tinputcon : input_controller");
            stream.WriteLine("\t\tport map");
            stream.WriteLine("\t\t(");
            stream.WriteLine("\t\t\tclock => clock,");
            stream.WriteLine("\t\t\ts_tart => s_tart,");
            stream.WriteLine("\t\t\treset => clear,");
            stream.WriteLine("\t\t\tfinish => finish,");
            stream.WriteLine("\t\t\tcontrol_out => sig_con_out");
            stream.WriteLine("\t\t);");
        }
        private void writeDesignControllerComponent(StreamWriter stream)
        {
            stream.WriteLine();
            stream.WriteLine("\tcomponent input_controller");
            stream.WriteLine("\t\tport");
            stream.WriteLine("\t\t(");
            stream.WriteLine("\t\t\tclock, reset, s_tart : IN std_logic;");
            stream.WriteLine("\t\t\tfinish : OUT std_logic;");
            stream.WriteLine($"\t\t\tcontrol_out : OUT std_logic_vector(0 to {ControllerBusBitWidth - 1})");
            stream.WriteLine("\t\t);");
            stream.WriteLine("\tend component;");
            stream.WriteLine("\tfor all : input_controller use entity work.input_controller(moore);");
        }
        private void writeDesignDataPath(StreamWriter stream, RegisterBase[] registers)
        {
            stream.WriteLine();
            stream.WriteLine("\tinputdp : input_dp");
            stream.WriteLine("\t\tport map");
            stream.WriteLine("\t\t(");
            foreach (var item in registers)
            {
                stream.WriteLine($"\t\t\t{item.Name} => {item.Name},");
            }
            foreach (var index in Enumerable.Range(0, ControllerBusBitWidth))
            {
                stream.WriteLine($"\t\t\tctrl({index}) => sig_con_out({index}),");
            }
            stream.WriteLine("\t\t\tclock => clock,");
            stream.WriteLine("\t\t\tclear => clear");
            stream.WriteLine("\t\t);");
        }
        private void writeDesignDataPathComponent(StreamWriter stream, RegisterBase[] registers)
        {
            stream.WriteLine();
            stream.WriteLine("\tcomponent input_dp");
            stream.WriteLine("\t\tport");
            stream.WriteLine("\t\t(");
            foreach (var group in registers
                .GroupBy(item => $"{(item is InputRegister ? "IN" : "OUT")} std_logic_vector({item.Bits - 1} downto 0)"))
            {
                stream.WriteLine($"\t\t\t{string.Join(", ", group.Select(reg => reg.Name))} : {group.Key};");
            }
            stream.WriteLine($"\t\t\tctrl : IN std_logic_vector(0 to {ControllerBusBitWidth - 1});");
            stream.WriteLine("\t\t\tclear, clock : IN std_logic");
            stream.WriteLine("\t\t);");
            stream.WriteLine("\tend component;");
            stream.WriteLine("\tfor all : input_dp use entity work.input_dp(rtl);");
        }

        /// <summary>
        /// Generates the VHDL file that takes the input and sends down the data path then maps the output to the outputs.
        /// </summary>
        public void SaveDesign(StreamWriter stream)
        {
            writeDesignComments(stream);

            stream.WriteLine();
            stream.WriteLine("library IEEE;");
            stream.WriteLine("use IEEE.std_logic_1164.all;");

            var registers = writeDesignEntity(stream);

            stream.WriteLine();
            stream.WriteLine("architecture rtl of input is");

            writeDesignControllerComponent(stream);

            writeDesignDataPathComponent(stream, registers);

            stream.WriteLine();
            stream.WriteLine($"\tsignal sig_con_out : std_logic_vector(0 to {ControllerBusBitWidth - 1});");

            stream.WriteLine();
            stream.WriteLine("begin");

            writeDesignController(stream);

            writeDesignDataPath(stream, registers);

            stream.WriteLine("end rtl;");
        }
    }
}
