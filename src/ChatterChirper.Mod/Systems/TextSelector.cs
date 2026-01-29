using System.Collections.Generic;
using ChatterChirper.Models;
using System;
using System.Linq;

namespace ChatterChirper.Systems
{
    public class TextSelector
    {
        private Dictionary<string, DateTime> lastSeen = new Dictionary<string, DateTime>();
        private Random random = new Random();

        public MessageDefinition SelectMessage(CityContext context, List<MessageDefinition> pool)
        {
            if (pool == null || pool.Count == 0 || context == null) return null;

            var matches = new List<MessageDefinition>();

            foreach (var msg in pool)
            {
                if (CheckConditions(msg, context))
                {
                    // Check Cooldown
                    if (!IsOnCooldown(msg))
                    {
                        matches.Add(msg);
                    }
                }
            }

            if (matches.Count == 0) return null;

            // Weighted Random Selection
            return SelectWeighted(matches);
        }

        private bool IsOnCooldown(MessageDefinition msg)
        {
             if (!lastSeen.ContainsKey(msg.id)) return false;
             
             // Time since last seen
             // In game we use SimulationManager.instance.m_currentFrameIndex or m_currentGameTime
             // For simplicity/portability, let's use DateTime Now as proxy, 
             // though in-game time is better. For MVP let's use wall clock.
             
             double secondsSince = (DateTime.Now - lastSeen[msg.id]).TotalSeconds;
             return secondsSince < msg.cooldown;
        }

        private MessageDefinition SelectWeighted(List<MessageDefinition> matches)
        {
            float totalWeight = 0;
            foreach(var m in matches) totalWeight += CalculateWeight(m);
            
            float roll = (float)(random.NextDouble() * totalWeight);
            
            float current = 0;
            foreach(var m in matches)
            {
                current += CalculateWeight(m);
                if (roll <= current)
                {
                    RecordSelection(m);
                    return m;
                }
            }
            
            // Fallback
            var final = matches.Last();
            RecordSelection(final);
            return final;
        }

        private void RecordSelection(MessageDefinition msg)
        {
            lastSeen[msg.id] = DateTime.Now;
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
                else if (key == "taxRateResidential") targetValue = context.TaxRateResidential;
                else if (key == "isDisasterActive") targetValue = context.IsDisasterActive ? 1.0f : 0.0f;
                else if (key == "crimeRate") targetValue = context.CrimeRate;
                else if (key == "fireHazard") targetValue = context.FireHazard;
                else if (key == "healthAvg") targetValue = context.HealthAvg;
                else if (key == "educationAvg") targetValue = context.EducationAvg;
                else if (key == "population") targetValue = context.Population;
                else if (key == "money") targetValue = context.Money;
                else if (key == "isNight") targetValue = context.IsNight ? 1.0f : 0.0f;
                else 
                {
                    // ModLogger.Debug("TextSelector: Unknown condition key " + key);
                    found = false;
                }

                if (found && !EvaluateCondition(opValue, targetValue)) 
                {
                    // Optional: Log failure reason
                    // ModLogger.Debug("Condition failed: " + key + " " + opValue + " (Actual: " + targetValue + ")");
                    return false;
                }
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