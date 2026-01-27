using System.Collections.Generic;
using ChatterChirper.Models;
using System;

namespace ChatterChirper.Systems
{
    public class TextSelector
    {
        public MessageDefinition SelectMessage(CityContext context, List<MessageDefinition> pool)
        {
            if (pool == null || pool.Count == 0 || context == null) return null;

            var matches = new List<MessageDefinition>();

            foreach (var msg in pool)
            {
                if (CheckConditions(msg, context))
                {
                    // T022: Apply Toxicity
                    // If toxicity is high, and message is severe, boost it (pretend weight)
                    // Since we return FIRST match, we should sort by (Weight * Multiplier) descending?
                    // For MVP Phase 4, let's just add to candidates.
                    matches.Add(msg);
                }
            }

            if (matches.Count == 0) return null;

            // Sort by effective weight
            matches.Sort((a, b) => {
                float wA = CalculateWeight(a);
                float wB = CalculateWeight(b);
                return wB.CompareTo(wA); // Descending
            });

            return matches[0];
        }
        
        private float CalculateWeight(MessageDefinition msg)
        {
             float w = msg.weight;
             if (msg.severity >= 4) w *= ModConfig.Instance.Toxicity;
             return w;
        }

        private bool CheckConditions(MessageDefinition msg, CityContext context)
        {
            if (msg.conditions == null) return true;

            foreach (var kvp in msg.conditions)
            {
                string key = kvp.Key;
                string opValue = kvp.Value; 

                float targetValue = 0;
                bool found = true;
                
                if (key == "trafficFlow") targetValue = context.TrafficFlow;
                else if (key == "happiness") targetValue = context.Happiness;
                else if (key == "unemployment") targetValue = context.Unemployment;
                else found = false;

                if (found && !EvaluateCondition(opValue, targetValue)) return false;
            }
            return true;
        }

        private bool EvaluateCondition(string opValue, float targetValue)
        {
            try
            {
                if (opValue.StartsWith("<"))
                {
                    float limit = float.Parse(opValue.Substring(1));
                    return targetValue < limit;
                }
                if (opValue.StartsWith(">"))
                {
                    float limit = float.Parse(opValue.Substring(1));
                    return targetValue > limit;
                }
            }
            catch 
            {
                // Parse error
                return false;
            }
            return false;
        }
    }
}