using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Tests.data;

namespace TacoLib
{
    public class TacoConfiguration
    {
        public int MaxBufferCount { get; set; } 
        public int DataSize { get; set; }
        public int DataPadding { get; set; } 
        public int DataCount { get; set; } 

        private int? _bufferSize;
        public int DataBufferSize => _bufferSize ??= (DataCount * DataSize) + 
            10 + 
            (Environment.Is64BitOperatingSystem ? 8 : 4);

        public int MessageSize { get; set; }
        public int MaxIdleMs { get; set; } 
        public string DataSource { get; set; }
        public string TacoStreamFileName { get; set; } 
        public int DataSourceBufferSize { get; set; } 
        public string AldlIoPath { get; set; } 
        public int MessageReaderFailedRetryDelayS { get; set; } 
        public int TacoStreamProcessRestartS { get; set; }

        public TacoGaugeLayout[] Layouts { get; set; } = new[]
        {
            new TacoGaugeLayout(TacoValueId.RPM),
            new TacoGaugeLayout(TacoValueId.SPEED),
            new TacoGaugeLayout(TacoValueId.COOLTMP),
            new TacoGaugeLayout(TacoValueId.LBPW)
            {
                Min = 0.0,
                Max = 7.0
            },
            new TacoGaugeLayout(TacoValueId.RBPW)
            {
                Min = 0.0,
                Max = 7.0
            },
            new TacoGaugeLayout(TacoValueId.BLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.LBLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.RBLM, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.LINT, GaugeType.Text),
            new TacoGaugeLayout(TacoValueId.RINT, GaugeType.Text),

        };

    }

    public enum TacoDataSource
    {
        StdIn,
    }
}
