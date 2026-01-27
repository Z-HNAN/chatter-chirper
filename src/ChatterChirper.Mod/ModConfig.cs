using System.IO;
using System.Xml.Serialization;

namespace ChatterChirper
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public float Toxicity { get; set; } = 1.0f;
        public float ReplaceProbability { get; set; } = 1.0f;

        public static ModConfig Instance { get; set; } = new ModConfig();
    }
}