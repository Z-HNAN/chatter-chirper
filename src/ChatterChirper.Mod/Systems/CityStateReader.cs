using ChatterChirper.Models;
// using ColossalFramework; 

namespace ChatterChirper.Systems
{
    public static class CityStateReader
    {
        public static CityContext GetCurrentContext()
        {
             var ctx = new CityContext();
             // Default values for robustness
             ctx.TrafficFlow = 100;
             ctx.Happiness = 100;
             
             // In actual game build:
             // ctx.TrafficFlow = Singleton<TrafficManager>.instance.m_averageTrafficFlow;
             
             return ctx;
        }
    }
}