namespace ChatterChirper.Models
{
    public class CityContext
    {
        public float TrafficFlow;       // 0-100. 100 is best.
        public int Happiness;           // 0-100.
        public int Unemployment;        // Percentage (e.g., 5 for 5%).
        public int TaxRateResidential;  // Percentage.
        public bool IsDisasterActive;
    }
}