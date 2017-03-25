using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synthesize.FileParsing
{
    public class AifFile
    {
        private const string registerNamePattern = @"[a-z\d_]+";
        private void addOperation(string line, int lineNumber)
        {
            const string sectionName = "operations";

            if (!Regex.IsMatch(line, $@"^{registerNamePattern}\s+{registerNamePattern}\s+\d+\s+{registerNamePattern}\s+{registerNamePattern}\s+{registerNamePattern}$", RegexOptions.IgnoreCase))
            {
                throw new ParsingError(sectionName, line, lineNumber, 0, "Incorrect Format");
            }

            var splitLine = Regex.Replace(line, @"\s+", "\t").Split('\t');

            int bits;
            if (!int.TryParse(splitLine[2], out bits) || bits < 1)
            {
                var lineIndex = line.Skip(splitLine[0].Length).TakeWhile(char.IsWhiteSpace).Count();
                throw new ParsingError(sectionName, line, lineNumber, lineIndex, "The number of bits is less then one.");
            }

            RegisterBase left;
            if (!Registers.TryGetValue(splitLine[3], out left))
            {
                throw new ParsingError(sectionName, line, lineNumber, line.IndexOf(splitLine[3], StringComparison.Ordinal), 
                    "Unable to find the left input register.");
            }

            RegisterBase right;
            if (!Registers.TryGetValue(splitLine[4], out right))
            {
                throw new ParsingError(sectionName, line, lineNumber, line.IndexOf(splitLine[4], StringComparison.Ordinal),
                    "Unable to find the right input register.");
            }

            RegisterBase output;
            if (!Registers.TryGetValue(splitLine[5], out output) ||
                output is InputRegister)
            {
                throw new ParsingError(sectionName, line, lineNumber, line.IndexOf(splitLine[5], StringComparison.Ordinal),
                    "Unable to find the output register.");
            }
            var outputWithParent = (RegisterWithParentBase)output;
            if (outputWithParent.Parent != null)
            {
                throw new ParsingError(sectionName, line, lineNumber, line.IndexOf(splitLine[5], StringComparison.Ordinal),
                    "The output register is already linked to another operation.");
            }

            var operation = outputWithParent.Parent = new Operation
            {
                Name = splitLine[0],
                Op = splitLine[1],
                Bits = bits,
                Left = left,
                Right = right,
                Output = outputWithParent
            };

            Operations.Add(operation.Name, operation);
        }
        private void addRegisters<TRegister>(string regType, string line, int lineNumber)
            where TRegister : RegisterBase, new()
        {
            if (!Regex.IsMatch(line, $@"^{regType}(\s+{registerNamePattern}\s+\d+)+$", RegexOptions.IgnoreCase))
            {
                throw new ParsingError(regType, line, lineNumber, 0, "Incorrect Format");
            }

            int bits;
            foreach (var item in Regex.Matches(line.Substring(regType.Length), $@"{registerNamePattern}\s+\d+", RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Select(match => new
                {
                    Match = match,
                    MatchStr = match.ToString()
                })
                .Select(match => new
                {
                    Match = match,
                    MatchSplit = Regex.Replace(match.MatchStr, @"\s+", "\t").Split('\t')
                })
                .Select(match => new
                {
                    Match = match,
                    Register = new TRegister
                    {
                        Name = match.MatchSplit[0],
                        Bits = int.TryParse(match.MatchSplit[1], out bits) ? bits : -1
                    }
                }))
            {
                if (item.Register.Bits < 1)
                {
                    var lineIndex = regType.Length + item.Match.Match.Match.Index + item.Register.Name.Length;
                    lineIndex += line.Skip(lineIndex).TakeWhile(char.IsWhiteSpace).Count();

                    throw new ParsingError(regType, line, lineNumber, lineIndex, "The number of bits is less then one.");
                }
                Registers.Add(item.Register.Name, item.Register);
            }
        }
        private void parse(IEnumerable<string> lines)
        {
            var ended = false;
            var inputsProcessed = false;
            var lineCount = 0;
            var outputsProcessed = false;
            var registersProcessed = false;
            foreach (var line in lines
                .Select(line => line.Trim()))
            {
                lineCount++;

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (string.Equals(line, "end", StringComparison.Ordinal))
                {
                    ended = true;
                    break;
                }

                if (!inputsProcessed)
                {
                    addRegisters<InputRegister>("inputs", line, lineCount);
                    inputsProcessed = true;
                }
                else if (!outputsProcessed)
                {
                    addRegisters<OutputRegister>("outputs", line, lineCount);
                    outputsProcessed = true;
                }
                else if (!registersProcessed)
                {
                    addRegisters<Register>("regs", line, lineCount);
                    registersProcessed = true;
                }
                else
                {
                    addOperation(line, lineCount);
                }
            }
            if (!inputsProcessed)
            {
                throw new ParsingError("inputs", null, 0, 0, "Missing the inputs.");
            }
            if (!outputsProcessed)
            {
                throw new ParsingError("outputs", null, 0, 0, "Missing the outputs.");
            }
            if (!registersProcessed)
            {
                throw new ParsingError("regs", null, 0, 0, "Missing the registers.");
            }
            if (!ended)
            {
                throw new ArgumentException("Missing the end tag.");
            }

            OperationTypes = Operations.Values
                .Select(op => op.Op)
                .Distinct()
                .ToArray();
        }
        private void traverseToInput(RegisterWithParentBase reg, List<string> pathFromOutput)
        {
            var parent = reg.Parent;

            if (MinCycles < pathFromOutput.Count)
            {
                MinCycles = pathFromOutput.Count;
            }

            var leftReg = parent.Left as RegisterWithParentBase;
            if (leftReg != null)
            {
                if (pathFromOutput.Contains(leftReg.Name))
                {
                    throw new CircularReferenceError(leftReg.Name);
                }
                traverseToInput(leftReg, new List<string>(pathFromOutput)
                {
                    leftReg.Name
                });
            }

            var rightReg = parent.Right as RegisterWithParentBase;
            if (rightReg != null)
            {
                if (pathFromOutput.Contains(rightReg.Name))
                {
                    throw new CircularReferenceError(rightReg.Name);
                }
                traverseToInput(rightReg, new List<string>(pathFromOutput)
                {
                    rightReg.Name
                });
            }
        }
        private void validate()
        {
            //find registers with missing parents
            var missingParentReg = Registers.Values.OfType<RegisterWithParentBase>().FirstOrDefault(reg => reg.Parent == null);
            if (missingParentReg != null)
            {
                throw new MissingParentError(missingParentReg.Name);
            }

            //find circular references
            foreach (var reg in Registers.Values.OfType<OutputRegister>())
            {
                traverseToInput(reg, new List<string>
                {
                    reg.Name
                });
            }
        }

        public AifFile(string file)
            : this(File.ReadAllLines(file))
        {
        }
        public AifFile(params string[] lines)
            : this((IEnumerable<string>)lines)
        {
        }
        public AifFile(IEnumerable<string> lines)
        {
            parse(lines);
            validate();
        }
        public Dictionary<string, Operation> Operations { get; } = new Dictionary<string, Operation>();
        public Dictionary<string, RegisterBase> Registers { get; } = new Dictionary<string, RegisterBase>();

        public int MinCycles { get; private set; }
        public override string ToString()
        {
            return 
                $"inputs {string.Join(" ", Registers.Values.OfType<InputRegister>())}{Environment.NewLine}" +
                $"outputs {string.Join(" ", Registers.Values.OfType<OutputRegister>())}{Environment.NewLine}" +
                $"regs {string.Join(" ", Registers.Values.OfType<Register>())}{Environment.NewLine}" +
                string.Join(Environment.NewLine, Operations.Values) + Environment.NewLine +
                "end";
        }
        public string[] OperationTypes { get; private set; }
    }
}
