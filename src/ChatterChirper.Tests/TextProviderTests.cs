using NUnit.Framework;
using ChatterChirper.Systems;
using ChatterChirper.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChatterChirper.Tests
{
    [TestFixture]
    public class TextProviderTests
    {
        [Test]
        public void Parse_ValidJson_ReturnsList()
        {
            string json = @"
            {
                ""messages"": [
                    {
                        ""id"": ""traffic_01"",
                        ""category"": ""traffic"",
                        ""severity"": 5,
                        ""conditions"": { ""trafficFlow"": ""<50"" },
                        ""texts"": [""Traffic is bad!""]
                    }
                ]
            }";

            // To be implemented: TextProvider.Parse
            List<MessageDefinition> result = TextProvider.Parse(json);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("traffic_01", result[0].id);
            Assert.AreEqual("traffic", result[0].category);
            Assert.AreEqual(5, result[0].severity);
            Assert.AreEqual("<50", result[0].conditions["trafficFlow"]);
            Assert.AreEqual("Traffic is bad!", result[0].texts[0]);
        }
    }
}