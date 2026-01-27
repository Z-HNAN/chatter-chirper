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
        
        [Test]
        public void SelectMessage_RespectsCooldown()
        {
            var msg = new MessageDefinition 
            { 
               id = "repeat_test",
               cooldown = 10,
               texts = new List<string> { "text" }
            };
            var pool = new List<MessageDefinition> { msg };
            var ctx = new CityContext();
            
            var selector = new TextSelector();
            
            // First select should succeed
            var r1 = selector.SelectMessage(ctx, pool);
            Assert.IsNotNull(r1);
            
            // Second select immediate should fail/return null or different (if pool had others)
            // Here pool only has one, so it should return null
            var r2 = selector.SelectMessage(ctx, pool);
            Assert.IsNull(r2);
        }

        [Test]
        public void SelectMessage_WeightedRandom()
        {
            var heavy = new MessageDefinition { id="heavy", weight=100.0f, texts=new List<string>{"H"} };
            var light = new MessageDefinition { id="light", weight=1.0f, texts=new List<string>{"L"} };
            var pool = new List<MessageDefinition> { heavy, light };
            
            var selector = new TextSelector();
            var ctx = new CityContext();
            
            int hCount = 0;
            for(int i=0; i<100; i++)
            {
               // clear cooldown/history for this test or use new selector
               var s = new TextSelector(); 
               var r = s.SelectMessage(ctx, pool);
               if(r.id == "heavy") hCount++;
            }
            
            Assert.Greater(hCount, 80); // Heavy should appear way more often
        }
    }
}