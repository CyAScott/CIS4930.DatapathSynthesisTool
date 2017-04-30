using System;
using System.Collections.Generic;
using System.Linq;
using Synthesize.FileParsing;

namespace Synthesize.Multiplexer
{
    public abstract class Multiplexer
    {
        /// <summary>
        /// The number of items to select from.
        /// </summary>
        public int ItemCount => ItemNames.Count();

        /// <summary>
        /// The number of bits for the multiplexer selecter value.
        /// </summary>
        public int SelectionBitSize { get; set; }
        
        /// <summary>
        /// All the items that can be selected.
        /// </summary>
        public abstract IEnumerable<IAmAUnit> Items { get; }

        /// <summary>
        /// The name of all the items this multiplexer can select.
        /// </summary>
        public abstract IEnumerable<string> ItemNames { get; }

        /// <summary>
        /// Returns the binary selection value and the name of the item selected.
        /// </summary>
        public abstract Tuple<string, string> GetSelectValueAndNameFrom(int cycle);
        
        /// <summary>
        /// The number of bits for output value.
        /// </summary>
        public abstract int OutputBitSize { get; }

        /// <summary>
        /// The name of the multiplexer.
        /// </summary>
        public abstract string Name { get; }
    }
}
