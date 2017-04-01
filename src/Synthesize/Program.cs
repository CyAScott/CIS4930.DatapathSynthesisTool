using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Synthesize.FileParsing;
using Synthesize.Scheduler;

namespace Synthesize
{
    public static class Program
    {
        public static ILogger Log = LogManager.GetLogger(nameof(Program));

        public static AifFile GetAifFile()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Provide a path to a AIF file. Enter nothing to exit.");

                Console.ForegroundColor = ConsoleColor.White;
                var path = Console.ReadLine();

                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                var returnValue = new AifFile(path);

                Log.Info(returnValue);

                return returnValue;
            }
            catch (Exception error)
            {
                Log.Error(error);
                return GetAifFile();
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


        public static void Main(string[] args)
        {
            try
            {
                var file = GetAifFile();
                if (file == null)
                {
                    return;
                }

                var schedule = GetSchedule(file);
                if (schedule == null)
                {
                    return;
                }


                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
            catch (Exception error)
            {
                Log.Error(error);
            }

        }
    }
}
