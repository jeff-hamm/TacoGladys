using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib.Tests
{
    public class ProccessTests : TestBase
    {
        private ITacoMessageParser _parser;
        private TacoMessageReaderRunner _reader;

        [OneTimeSetUp]
        public void Setup()
        {
            Trace.WriteLine("Setting Up");

            IConfiguration configuration = new ConfigurationBuilder()
                .Build();
            IServiceCollection services = new ServiceCollection()
                .AddLogging(b => { b.AddConsole(); });
            services.AddTacoServices(null);
            Provider = services.BuildServiceProvider();
            _parser = base.GetFormatter();
            _reader = Provider.GetService<TacoMessageReaderRunner>();
        }

        [Test]
        public async Task TestDefinitionRead()
        {
            var defs = await Provider.GetService<TacoDefinitions>().GetDefinitionsAsync().ConfigureAwait(false);
            Assert.IsNotNull(defs);
            Assert.AreEqual(defs.Length, 69);
   //         _cts = new CancellationTokenSource(10000);
  //          _reader.StartReading();
            await Task.Delay(5000).ConfigureAwait(false);
        }

//          [Test]
/*        public async Task TestBeginStream()
        {
            var stream = (await Provider.GetService<ITacoStreamFactory>()
                .GetOrOpenAsync(TacoStreamType.Message, CancellationToken.None)) as ProcessTacoStream;

            var p = stream.Process;
            Assert.IsNotNull(p);
            Assert.IsFalse(p.HasExited);
            Assert.IsTrue(p.StartInfo.RedirectStandardOutput);
            Assert.IsTrue(p.StandardOutput.BaseStream.CanRead);
            Assert.IsTrue(_reader.IsRunning);
            var msg = await _reader.ReadMessageAsync(default);
            Assert.IsTrue(_reader.IsRunning);
            Assert.IsNotNull(msg);
            var msg2 = await _reader.ReadMessageAsync(default);
            Assert.IsNotNull(msg);
        }*/
/*
        [TearDown]
        public void TearDown()
        {
            _cts.Cancel();
            _reader.Dispose();
        }
  */      
        /*
        private class TestAldlIoTacoMessageReader : ProcessTacoMessageReader
        {
            public TestAldlIoTacoMessageReader(TacoConfiguration config, AldlIoMessageFormatter formatter) : base(config, formatter)
            {
            }

            public Task<Stream> BeginStreamAsyncTest(CancellationToken token) =>
                BeginStreamAsync(token);

            public void BeginProcessTest() =>
                base.BeginProcess();
        }*/

    }
}
