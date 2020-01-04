using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using TacoLib.Extensions;
using TacoLib.Interop;

namespace TacoLib.Data
{
    public class AldlStreamDataSource : ITacoDataSource
    {
        private readonly ILogger<AldlStreamDataSource> _logger;
        private readonly TacoDefinitions _definitions;
        private readonly TacoMessageReaderRunner _messageReader;

        public AldlStreamDataSource(
            TacoDefinitions definitions, 
            TacoMessageReaderRunner messageReader, 
            ILogger<AldlStreamDataSource> logger)
        {
            _logger = logger;
            _definitions = definitions;
            _messageReader = messageReader;
        }

        public Task<TacoDefinition[]> GetDefinitionsAsync(CancellationToken token=default) =>
            _definitions.GetDefinitionsAsync(token);

        public Task<TacoMessage> ReadMessageAsync(CancellationToken token=default) =>
            _messageReader.GetMessageAsync(token);


    }
}