using NUnit.Framework;
using ChatterChirper.Systems;
using ChatterChirper.Models;
using System.Collections.Generic;

namespace ChatterChirper.Tests
{
    [TestFixture]
    public class TextSelectorTests
    {
        [Test]
        public void SelectMessage_HighTraffic_ReturnsTrafficMessage()
        {
            // Arrange
            var context = new CityContext
            {
                TrafficFlow = 40.0f, // Low flow = high traffic
                Happiness = 100
            };

            var pool = new List<MessageDefinition>
            {
                new MessageDefinition
                {
                    id = "traffic_msg",
                    category = "traffic",
                    conditions = new Dictionary<string, string> { { "trafficFlow", "<50" } },
                    texts = new List<string> { "Bad traffic" }
                },
                new MessageDefinition
                {
                    id = "happy_msg",
                    category = "praise",
                    conditions = new Dictionary<string, string> { { "happiness", ">90" } },
                    texts = new List<string> { "So happy" }
                }
            };

            var selector = new TextSelector();

            // Act
            // We assume the selector prioritizes based on specificity or order, or first match.
            // Let's assume it checks conditions.
            MessageDefinition result = selector.SelectMessage(context, pool);

            // Assert
            // Since both conditions match (Flow < 50 AND Happiness > 90), 
            // the selector strategy determines which one. 
            // If "Traffic" is considered a negative/priority event, it might pick traffic.
            // Or if random.
            Assert.IsNotNull(result);
            // This test is fragile until we define priority. 
            // Let's make ONLY traffic match to test basic filtering.
            
            context.Happiness = 50; // Make happy nsg NOT match
            result = selector.SelectMessage(context, pool);
            Assert.AreEqual("traffic_msg", result.id);
        }
        
        [Test]
        public void SelectMessage_NoMatch_ReturnsNull()
        {
            var context = new CityContext { TrafficFlow = 100 };
            var pool = new List<MessageDefinition>
            {
                 new MessageDefinition
                {
                    id = "traffic_msg",
                    conditions = new Dictionary<string, string> { { "trafficFlow", "<50" } },
                    texts = new List<string> { "Bad traffic" }
                }
            };
            
            var selector = new TextSelector();
            Assert.IsNull(selector.SelectMessage(context, pool));
        }
    }
}