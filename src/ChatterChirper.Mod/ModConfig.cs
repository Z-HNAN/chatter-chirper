using System.IO;
using System.Xml.Serialization;

namespace ChatterChirper
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public int ReplaceChance { get; set; } = 100;
        
        public static ModConfig Instance { get; set; } = new ModConfig();
    }
}
