using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TacoLib.Data;
using TacoLib.Interface;
using TacoLib.Tests.data;

namespace TacoLib.Tests
{/*
    public class GaugeBuilderTests : TestBase
    {
        private ITacoMessageParser _parser;
        private Task<TacoMessage> _message;
        private TacoGaugeLayout[] _layout;

        [OneTimeSetUp]
        public void Setup()
        {
            _parser = base.GetFormatter();
        _layout = new []
        {
            new TacoGaugeLayout(TacoValueId.RPM),
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
        };
        }

        [Test]
        public async Task TestGauges()
        {
            var defs = await base.ReadDefinitions(_parser);
            var gauges = new TacoGauges(_layout,defs);
            var values = gauges.GetLayoutValues(await base.ReadMessage(_parser)).ToArray();
            Assert.AreEqual(values.Length, _layout.Length);
            foreach (var value in values)
            {
//                Assert.AreEqual(value.Item1.Definition, value.Item2.Definition);
            }
        }


    }*/
}
