using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Synthesize.Allocation;
using Synthesize.DataPath;
using Synthesize.Extensions;
using Synthesize.FileParsing;
using Synthesize.Multiplexer;
using Synthesize.Scheduler;

namespace Synthesize
{
    public static class Program
    {
        public static ILogger Log = LogManager.GetLogger(nameof(Program));

        public static AifFile GetAifFile(out string name)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Provide a path to a AIF file. Enter nothing to exit.");

                Console.ForegroundColor = ConsoleColor.White;
                var path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    name = null;
                    return null;
                }

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                var returnValue = new AifFile(path);

                Log.Info(returnValue);

                name = Path.GetFileNameWithoutExtension(path);

                return returnValue;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetAifFile(out name);
            }
        }
        public static Dictionary<string, int> GetResources(AifFile file)
        {
            try
            {
                var returnValue = new Dictionary<string, int>();
                foreach (var op in file.OperationTypes)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Enter a resource constraint for {op} that is greater then or equal to 1 (enter nothing to exit):");

                    Console.ForegroundColor = ConsoleColor.White;
                    var line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        return null;
                    }

                    var value = Convert.ToInt32(line);
                    if (value < 1)
                    {
                        throw new ArgumentException($"The resource constraint for {op} must be greater then or equal to 1.");
                    }

                    returnValue[op] = value;
                }

                Log.Info("Resource Constraint: " + string.Join(", ", returnValue.Select(pair => $"{pair.Key} = {pair.Value}")));

                return returnValue;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetResources(file);
            }
        }
        public static IEnumerable<Dictionary<string, string>> GetTestCases(DataPathGenerator dataPathGenerator)
        {
            while (true)
            {
                if (!agreeToCreateTestCase())
                {
                    yield break;
                }

                var returnValue = new Dictionary<string, string>();

                var file = dataPathGenerator.AifFile;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter the values for the inputs:");
                foreach (var reg in file.Registers.Values.OfType<InputRegister>())
                {
                    returnValue[reg.Name] = GetInputForReg(reg);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter the expected values for the outputs:");
                foreach (var reg in file.Registers.Values.OfType<OutputRegister>())
                {
                    returnValue[reg.Name] = GetInputForReg(reg);
                }

                yield return returnValue;
            }
        }
        public static SchedulerBase GetSchedule(AifFile file)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Select a scheduler algorithm:");
                Console.WriteLine("\tA) List Scheduler (resource constraints required)");
                Console.WriteLine("\tB) Integer Linear Programing Scheduler (time constraint required)");
                Console.WriteLine("\tC) Exit");

                Console.ForegroundColor = ConsoleColor.White;
                var selection = Console.ReadLine()?.Trim().ToUpper().Substring(0, 1) ?? "C";

                SchedulerBase schedule;
                switch (selection)
                {
                    case "A":
                        var resources = GetResources(file);
                        schedule = resources == null ? null : new ListScheduler(file, resources);
                        break;
                    case "B":
                        var latency = GetLatency(file);
                        schedule = latency == null ? null : new IlpScheduler(file, latency.Value);
                        break;
                    case "C":
                        schedule = null;
                        break;
                    default:
                        throw new ArgumentException("Unable to parse answer: " + selection);
                }
                
                schedule?.BuildSchedule();

                return schedule;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetSchedule(file);
            }
        }
        public static StreamWriter GetSaveStream(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return File.AppendText(file);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"The file \"{file}\" exists already. Enter nothing to exit or enter yes to overwrite the file.");

                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(line))
                {
                    return null;
                }

                if (line.IndexOf('y') != -1)
                {
                    File.Delete(file);
                    var stream = File.AppendText(file);
                    stream.NewLine = "\n";
                    return stream;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Enter a save path for the file. Enter nothing to exit.");

                Console.ForegroundColor = ConsoleColor.White;
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    return null;
                }

                if (!File.Exists(line))
                {
                    return File.AppendText(line);
                }

                return GetSaveStream(line);
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetSaveStream(file);
            }
        }
        public static bool agreeToCreateTestCase()
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Do you want to add a test case (yes or no)?");

                    Console.ForegroundColor = ConsoleColor.White;
                    var answer = Console.ReadLine()?.Trim().ToLower() ?? "";

                    if (answer == "no")
                    {
                        return false;
                    }

                    if (answer != "yes")
                    {
                        throw new ArgumentException();
                    }

                    return true;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid answer.");
                }
            }
        }
        public static int? GetLatency(AifFile file)
        {
            try
            {
                var minLatency = file.MinCycles.Values.Max();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Enter a latency constraint greater then or equal to {minLatency} (enter nothing to exit):");

                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    return null;
                }

                var returnValue = Convert.ToInt32(line);
                if (returnValue < minLatency)
                {
                    throw new ArgumentException($"The latency must be greater then or equal to {minLatency}.");
                }

                Log.Info("Latency Constraint: " + returnValue);

                return returnValue;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetLatency(file);
            }
        }
        public static string GetInputForReg(RegisterBase reg)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Enter the base 10 value for {reg.Name}:");

                Console.ForegroundColor = ConsoleColor.White;
                var inputStr = Console.ReadLine()?.Trim();

                long inputInt;
                if (!long.TryParse(inputStr, out inputInt))
                {
                    throw new ArgumentException("Please enter an integer number.");
                }

                var bits = reg.Bits;
                var max = Math.Pow(2, bits - 1) - 1;
                var min = -Math.Pow(2, bits - 1);

                if (inputInt > max)
                {
                    throw new ArgumentException($"The max number allowed for this register is {max}.");
                }

                if (inputInt < min)
                {
                    throw new ArgumentException($"The minimum number allowed for this register is {min}.");
                }

                if (inputInt < 0)
                {
                    //convert the negative number to 2's complement

                    inputInt++;
                    inputInt *= -1;

                    return "1" + Convert.ToString(inputInt, 2).PadLeft(bits - 1, '0')
                        .Replace('0', '-')
                        .Replace('1', '_')
                        .Replace('-', '1')
                        .Replace('_', '0');
                }

                return Convert.ToString(inputInt, 2).PadLeft(bits, '0');
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetInputForReg(reg);
            }
        }
        public static string GetSaveTo()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Provide a folder path to save the VHDL files. Enter nothing to exit.");

                Console.ForegroundColor = ConsoleColor.White;
                var path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException(path);
                }

                return path;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetSaveTo();
            }
        }
        
        public static void Main(string[] args)
        {
            StreamWriter controller = null, dataPath = null, design = null, testBench = null;
            try
            {
                string fileName;
                var file = GetAifFile(out fileName);
                if (file == null)
                {
                    return;
                }

                //1. Operation Scheduling:
                var schedule = GetSchedule(file);
                if (schedule == null)
                {
                    return;
                }

                //2. Functional Unit Allocation and Binding:
                var functionalUnits = new FunctionalUnitAllocator(schedule);

                //3. Register Allocation and Binding
                var registers = new RegisterAllocator(functionalUnits);

                //4. Multiplexer Generation:
                var multiplexorGenerator = new MultiplexerGenerator(registers);

                //5. Datapath Generation in VHDL:
                var dataPathGenerator = new DataPathGenerator(multiplexorGenerator);

                //6. Get test cases
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var expression in file.AsExpressions)
                {
                    Console.WriteLine(expression);
                }
                var testCases = GetTestCases(dataPathGenerator).ToArray();

                var saveFolder = GetSaveTo();
                if (saveFolder == null)
                {
                    return;
                }

                //save generation
                dataPath = GetSaveStream(Path.Combine(saveFolder, fileName + "_dp.vhd"));
                if (dataPath == null)
                {
                    return;
                }
                controller = GetSaveStream(Path.Combine(saveFolder, fileName + "_con.vhd"));
                if (controller == null)
                {
                    return;
                }
                design = GetSaveStream(Path.Combine(saveFolder, fileName + "_des.vhd"));
                if (design == null)
                {
                    return;
                }
                if (testCases.Length > 0)
                {
                    testBench = GetSaveStream(Path.Combine(saveFolder, fileName + "_tb.vhd"));
                    if (testBench == null)
                    {
                        return;
                    }
                }

                using (controller)
                {
                    dataPathGenerator.SaveController(controller);
                }

                foreach (var codeFile in functionalUnits.Units
                    .Select(unit => unit.VhdlCodeFile)
                    .Concat(new []
                    {
                        "c_multiplexer",
                        "c_register"
                    })
                    .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var savePath = Path.Combine(saveFolder, codeFile + ".vhd");
                    if (!File.Exists(savePath))
                    {
                        using (var stream = File.CreateText(savePath))
                        {
                            stream.WriteVhdlFile(codeFile);
                        }
                    }
                }
                using (dataPath)
                {
                    dataPathGenerator.SaveDataPath(dataPath);
                }
                using (design)
                {
                    dataPathGenerator.SaveDesign(design);
                }
                if (testCases.Length > 0)
                {
                    using (testBench)
                    {
                        dataPathGenerator.SaveTestBench(testBench, testCases);
                    }
                }
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
            catch (Exception error)
            {
                Log.Error(error);

                dataPath?.Dispose();
                design?.Dispose();
                controller?.Dispose();
                testBench?.Dispose();
            }
        }
    }
}
