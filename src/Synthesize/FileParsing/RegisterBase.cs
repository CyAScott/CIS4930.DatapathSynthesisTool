﻿namespace Synthesize.FileParsing
{
    public abstract class RegisterBase
    {
        public override string ToString()
        {
            return $"{Name} {Bits}";
        }
        public string Name { get; set; }
        public int Bits { get; set; }
    }
}
