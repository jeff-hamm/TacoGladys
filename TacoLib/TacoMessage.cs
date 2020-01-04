using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TacoLib.Interop;

namespace TacoLib
{
    public class TacoMessage : IEnumerable<(TacoDefinition definition,object value)>
    {
        private readonly TacoDefinitions _definitions;

        public TacoMessage(TacoDefinitions definitions)
        {
            _definitions = definitions;
        }

        public IReadOnlyCollection<TacoDefinition> Definitions => _definitions.GetDefinitionsSync();
        public int[] IntValues { get; internal set; }
        public object this[int index] => _definitions[index].ValueFromInt(IntValues[index]);
        public object this[TacoDefinition def] => def.ValueFromInt(IntValues[def.Index]);

        public IEnumerator<(TacoDefinition definition, object value)> GetEnumerator() =>
            Definitions.Select(d => (d, this[d])).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}