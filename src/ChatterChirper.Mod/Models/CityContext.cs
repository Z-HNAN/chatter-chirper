namespace ChatterChirper.Models
{
    public class CityContext
    {
        // Core Stats
        public float TrafficFlow;       // 0-100. 100 is best.
        public int Happiness;           // 0-100.
        
        // Economy & Pop
        public int Unemployment;        // Percentage (e.g., 5 for 5%).
        public int TaxRateResidential;  // Percentage.
        public int Population;          // Total count
        
        // Services (0-100)
        public int CrimeRate;
        public int FireHazard;
        public int HealthAvg;
        public int EducationAvg;
        
        // Resources (0=Critical, 100=Full)
        public int ElectricityAvailability; 
        public int WaterAvailability;
        public int GarbageStatus;
        
        // Economy
        public long Money;

        // Environment
        public float TimeOfDay;         // 0.0 - 24.0
        public bool IsNight;
        public bool IsDisasterActive;
        
        public override string ToString()
        {
            return string.Format(
                "Time:{0:F1}({1}) Pop:{2} Money:{3} | Hap:{4} UnEmp:{5}% Tax:{6}% | Traf:{7:F0}% Crime:{8}% Fire:{9}% | Hlth:{10} Edu:{11} Garb:{12}% | Elec:{13}% Water:{14}% | Disaster:{15}", 
                TimeOfDay, IsNight ? "Night" : "Day", Population, Money,
                Happiness, Unemployment, TaxRateResidential,
                TrafficFlow, CrimeRate, FireHazard,
                HealthAvg, EducationAvg, GarbageStatus,
                ElectricityAvailability, WaterAvailability,
                IsDisasterActive
            );
        }
    }
}