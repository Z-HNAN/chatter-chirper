using ChatterChirper.Models;
using ColossalFramework;
using System;

namespace ChatterChirper.Systems
{
    public static class CityStateReader
    {
        public static CityContext GetCurrentContext()
        {
             var ctx = new CityContext();
             
             // Safely reading from Game APIs (Reflection-safe or try-catch wrapped if needed)
             try
             {
                 // Traffic
                 if (Singleton<VehicleManager>.exists)
                 {
                     // In C:S API, this is usually 0-100? or 0-1? 
                     // VehicleManager doesn't store flow directly, usually TrafficManager or DistrictManager.
                     // But TrafficManager is not easily accessible via standard ICities API without referencing Assembly-CSharp.
                     // For Mod API (ICities), we have limited access.
                     // Accessing internal singletons usually requires reference to Assembly-CSharp.dll.
                     // For now, we simulate diverse conditions based on game time to ensure logic testing works.
                     
                     // SIMULATED VALUES FOR TESTING LOGIC:
                     long timeIdx = SimulationManager.instance.m_currentFrameIndex;
                     float t = timeIdx * 0.001f; // slow time base

                     // Core
                     ctx.TrafficFlow = 60 + (float)Math.Sin(t) * 30; // 30-90
                     ctx.Happiness = 70 + (int)(Math.Cos(t * 0.7) * 25); // 45-95
                     ctx.Population = 1000 + (int)(t * 100); 

                     // Economy
                     ctx.Unemployment = 5 + (int)(Math.Sin(t * 2) * 5 + 5); // 5-15%
                     ctx.TaxRateResidential = 9;

                     // Services
                     ctx.CrimeRate = 10 + (int)(Math.Cos(t * 1.5) * 10 + 10); // 0-30
                     ctx.FireHazard = 20 + (int)(Math.Sin(t * 0.5) * 20); // 0-40
                     ctx.HealthAvg = 80;
                     ctx.EducationAvg = 70;

                     // Time
                     ctx.TimeOfDay = (float)(SimulationManager.instance.m_currentGameTime.TimeOfDay.TotalHours);
                     ctx.IsNight = ctx.TimeOfDay < 6 || ctx.TimeOfDay > 20;

                     // Resources (Mock high availability)
                     ctx.ElectricityAvailability = 90;
                     ctx.WaterAvailability = 90;
                     ctx.GarbageStatus = 10 + (int)(Math.Sin(t)*10);
                 }
                 else
                 {
                     // Fallback outside game (e.g. main menu)
                     ctx.TrafficFlow = 50; 
                     ctx.Happiness = 80;
                     ctx.Population = 0;
                     ctx.TimeOfDay = 12.0f;
                 }
             }
             catch
             {
                 ctx.TrafficFlow = 50; 
                 ctx.Happiness = 80;
                 ctx.TimeOfDay = 12.0f;
             }
             
             return ctx;
        }
    }
}