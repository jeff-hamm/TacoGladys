using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib.Interface
{
    public enum GaugeSize
    {
        Small,
        Medium,
        Large
    }
    public class TacoGaugeLayout
    {
        public TacoGaugeLayout()
        {
        }

        public TacoGaugeLayout(GaugeSize? size)
        {
            Size = size;
        }
        public TacoGaugeLayout(GaugeType? type)
        {
            Type = type;
        }
        public TacoGaugeLayout(TacoValueId id, GaugeSize? size) : this(size)
        {
            DefValueId = id;
        }
        public TacoGaugeLayout(TacoValueId id, GaugeType? type=null) : this(type)
        {
            DefValueId = id;
        }

        public TacoGaugeLayout(string id, GaugeSize? size): this(size)
        {
            Id = id;
        }

        public TacoValueId? DefValueId { get;set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Width { get; set; }
        public GaugeType? Type { get;set; }
        public object Min { get; set; }
        public object Max { get; set; }
        public GaugeSize? Size { get; set; }

        [JsonIgnore]
        public (int? X, int? Y) Position
        {
            get => (0, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            } 
        }

        [JsonIgnore]
        public TacoGaugeLayout Previous { get;set; }
        [JsonIgnore]
        public TacoGaugeLayout Next { get; set; }
        [JsonIgnore]
        public TacoDefinition Definition { get; set; }
    }
}
