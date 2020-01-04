using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TacoLib.Data;
using TacoLib.Interop;

namespace TacoLib.Tests
{/*
    [TestFixture]
    public class AldlFormatterTests : TestBase
    {
        private Stream _dataSource;
        private AldlIoMessageParser _parser;

        [OneTimeSetUp]
        public void Setup()
        {
            var provider = ConfigureAndBuild();
            _parser = provider.GetService<AldlIoMessageParser>();
        }


        [SetUp]
        public void FormatterSetup()
        {
            _dataSource = System.IO.File.OpenRead("./data/demolog.bin");
        }
        [Test]
        public async Task TestFormatter()
        {
            await _parser.ParseAldlDefinitions(_dataSource);
            Assert.IsNotNull(_parser.Defintitions);
            Assert.IsNotEmpty(_parser.Defintitions);
            foreach (var def in _parser.Defintitions)
            {
                Enum.IsDefined(typeof(AldlDataType), (int) def.type);
            }

            var buffer = new TacoMessageBuffer()
            {
                Data = new byte[_parser.BufferSize],
                DataLength = _parser.BufferSize,
                StartTime = DateTime.Now,
                LastUpdated = DateTime.Now.Ticks
            };
            while((buffer.DataLength = _dataSource.Read(buffer.Data, 0, _parser.BufferSize)) == _parser.BufferSize)
            {
                var message = _parser.ParseMessage(buffer, DateTime.Now.Ticks);
                Assert.IsNotNull(message.Values);
                Assert.AreEqual(message.Values.Length, _parser.Defintitions.Length);
            }
        }
        [TearDown]
        public void TearDown()
        {
            _dataSource?.Dispose();
            _dataSource = null;
        }
    }*/
}

