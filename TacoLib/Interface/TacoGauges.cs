using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Interop;

namespace TacoLib.Tests.data
{
    public class TacoGauges
    {
        public TacoGauges(IEnumerable<TacoGaugeLayout> layouts, TacoDefinition[] definitions)
        {
            Layout = ConfigureLayouts(layouts, definitions).ToArray();
        }
        public IEnumerable<TacoGaugeLayout> ConfigureLayouts(IEnumerable<TacoGaugeLayout> layouts, TacoDefinition[] definitions)
        {
            foreach(var layout in layouts)
                yield return ConfigureLayout(layout,definitions);
        }

        public TacoGaugeLayout ConfigureLayout(TacoGaugeLayout layout, TacoDefinition[] definitions)
        {
            
            var def = definitions?.FirstOrDefault(d => (layout.DefValueId.HasValue && d.ValueId == layout.DefValueId)
                                                      || (!String.IsNullOrEmpty(layout.Id) && d.Id == layout.Id))??default;
          //  if (String.IsNullOrEmpty(def.Id)) throw new Exception($"{layout.DefValueId}, {layout.Id}, {definitions?.Length}");
            return ConfigureLayout(layout, def);
        }
        public TacoGaugeLayout ConfigureLayout(TacoGaugeLayout layout, TacoDefinition d)
        {
            layout.Id = d.Id;
            layout.DefValueId = d.ValueId;
            layout.Definition = d;
            layout.Max ??= d.MaxValue();
            layout.Min ??= d.MinValue();
            layout.Name ??= d.Description;
            layout.Type ??= (d.type == AldlDataType.ALDL_BOOL ? GaugeType.Boolean : GaugeType.Radial);
            return layout;
        }

        public TacoGaugeLayout[] Layout { get; }
    }

}
