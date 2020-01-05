using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Interop;

namespace TacoLib.Tests.data
{
    public class TacoGauges
    {
        private TacoDefinition[] _definitions;
        private readonly ITacoDataSource _dataSource;

        public TacoGauges(TacoConfiguration config, ITacoDataSource dataSource)
        {
            _dataSource = dataSource;
            _layouts = config.Layouts;
        }
        public TacoGauges(IEnumerable<TacoGaugeLayout> layouts, TacoDefinition[] definitions)
        {
            _definitions = definitions;
            _layouts = ConfigureLayouts(layouts).ToArray();
        }

        public void UpdateLayout(IEnumerable<TacoGaugeLayout> layouts)
        {
            _layouts = layouts.ToArray();
            _configured = false;
        }

        private bool _configured = false;
        internal IEnumerable<TacoGaugeLayout> ConfigureLayouts(IEnumerable<TacoGaugeLayout> layouts)
        {
            foreach(var layout in layouts)
                yield return ConfigureLayout(layout);
            _configured = true;
        }

        internal TacoGaugeLayout ConfigureLayout(TacoGaugeLayout layout)
        {
            if(_definitions == null) throw new ArgumentException(nameof(_definitions));
            var def = _definitions?.FirstOrDefault(d => (layout.DefValueId.HasValue && d.ValueId == layout.DefValueId)
                                                      || (!String.IsNullOrEmpty(layout.Id) && d.Id == layout.Id))??default;
          //  if (String.IsNullOrEmpty(def.Id)) throw new Exception($"{layout.DefValueId}, {layout.Id}, {definitions?.Length}");
            return ConfigureLayout(layout, def);
        }
        internal TacoGaugeLayout ConfigureLayout(TacoGaugeLayout layout, TacoDefinition d)
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

        public async Task<TacoGaugeLayout[]> EnsureConfiguredAsync(CancellationToken token=default)
        {
            if (_definitions == null)
                _definitions = await _dataSource.GetDefinitionsAsync(token);
            if(!_configured)
                _layouts = ConfigureLayouts(_layouts).ToArray();
            return _layouts;
        }

        private TacoGaugeLayout[] _layouts;

        public TacoGaugeLayout[] Layout
        {
            get
            {
                if(_definitions == null)
                    return Task.Run(() => EnsureConfiguredAsync(CancellationToken.None)).GetAwaiter().GetResult();
                if(!_configured)
                    _layouts = ConfigureLayouts(_layouts).ToArray();
                return _layouts;
            }
        }
    }

}
