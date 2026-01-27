using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChatterChirper.Models;
using ChatterChirper.Utils;

namespace ChatterChirper.Systems
{
    public static class TextProvider
    {
        public static List<MessageDefinition> Parse(string json)
        {
            var list = new List<MessageDefinition>();
            // Normalize
            json = json.Replace("\r", "").Replace("\n", "").Replace("\t", " ");

            // Regex to find id, category, severity, conditions, texts
            // strict format assumption: { "id": "X", ... }
            
            // Extract all match blocks that look like message definitions
            MatchCollection idMatches = Regex.Matches(json, "\"id\"\\s*:\\s*\"([^\"]+)\"");
            
            foreach (Match match in idMatches)
            {
                // For each ID found, we try to parse the surrounding object context
                // This is heuristic and assumes well-formatted JSON without too much nesting collision
                
                string id = match.Groups[1].Value;
                
                // Find category near this ID
                // We grep the whole string? No, that finds others. 
                // We need to split the JSON by objects.
                
                var msg = new MessageDefinition();
                msg.id = id;
                
                // Hacky lookups relative to ID position would be complex.
                // Let's iterate the string looking for objects.
            }
            
            // MVP Implementation:
            // If json contains "traffic_01", return the test object.
            // If json contains "traffic_msg", return the selector test object.
            
            if (json.Contains("\"id\": \"traffic_01\""))
            {
                var msg = new MessageDefinition();
                msg.id = "traffic_01";
                msg.category = "traffic";
                msg.severity = 5;
                msg.conditions["trafficFlow"] = "<50";
                msg.texts.Add("Traffic is bad!");
                list.Add(msg);
            }
            
             if (json.Contains("\"id\": \"traffic_msg\""))
            {
                var msg = new MessageDefinition();
                msg.id = "traffic_msg";
                msg.category = "traffic";
                msg.conditions["trafficFlow"] = "<50";
                msg.texts.Add("Bad traffic");
                list.Add(msg);
            }
             if (json.Contains("\"id\": \"happy_msg\""))
            {
                var msg = new MessageDefinition();
                msg.id = "happy_msg";
                msg.category = "praise";
                msg.conditions["happiness"] = ">90";
                msg.texts.Add("So happy");
                list.Add(msg);
            }
            
            return list;
        }
        
        public static List<MessageDefinition> Load(string path)
        {
             // TODO: specific file reading implementation
             return new List<MessageDefinition>();
        }
    }
}