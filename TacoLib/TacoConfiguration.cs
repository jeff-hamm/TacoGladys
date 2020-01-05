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
        public int MaxBufferCount { get; set; } = 1000;
        public int DataSize { get; set; } = 4;
        public int DataPadding { get; set; } = 8 + 10;
        public int DataCount { get; set; } = 69;

        private int? _bufferSize;
        public int DataBufferSize => _bufferSize ??= (DataCount * DataSize) + DataPadding;

        public int MessageSize { get; set; } = 100;
        public int MaxIdleMs { get; set; } = 30 * 1000;
        public string DataSource { get; set; } = "TacoLib.StdInDataSource";
        public string TacoStreamFileName { get; set; } = "./data/demolog.bin";
        public int DataSourceBufferSize { get; set; } = 4096;
        public string AldlIoPath { get; set; } = 
            Environment.OSVersion.Platform == PlatformID.Unix ? "/mnt/c/Users/jhamm/OneDrive/Projects/TacoGladys/aldl-1.5/aldl-dummy" : @"C:\Users\jhamm\OneDrive\Projects\TacoGladys\Web\RpiBlazor\AldlDummy\bin\Debug\netcoreapp3.1\AldlDummy.exe";
        public int MessageReaderFailedRetryDelayS { get; set; } = 5;
        public int TacoStreamProcessRestartS { get; set; } = 1;

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
