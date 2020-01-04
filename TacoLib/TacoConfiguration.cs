using System;
using System.Collections.Generic;
using System.Text;

namespace TacoLib
{
    public class TacoConfigurationService
    {
        public TacoConfigurationService(TacoConfiguration config)
        {

        }
    }

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
    }

    public enum TacoDataSource
    {
        StdIn,
    }
}
