using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Synthesize.Allocation;
using Synthesize.DataPath;
using Synthesize.FileParsing;
using Synthesize.Multiplexor;
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
                    return File.AppendText(file);
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
                functionalUnits.Allocate();

                //3. Register Allocation and Binding
                var registers = new RegisterAllocator(functionalUnits);
                registers.Allocate();

                //4. Multiplexor Generation:
                var multiplexorGenerator = new MultiplexorGenerator(registers);

                //5. Datapath Generation in VHDL:
                var dataPathGenerator = new DataPathGenerator(multiplexorGenerator);
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
                testBench = GetSaveStream(Path.Combine(saveFolder, fileName + "_tb.vhd"));
                if (testBench == null)
                {
                    return;
                }

                using (dataPath)
                {
                    dataPathGenerator.SaveDataPath(dataPath);
                }
                using (controller)
                {
                    dataPathGenerator.SaveController(controller);
                }
                using (design)
                {
                    dataPathGenerator.SaveDesign(design);
                }
                using (testBench)
                {
                    dataPathGenerator.SaveTestBench(testBench);
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
