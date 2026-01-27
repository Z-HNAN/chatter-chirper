using System.IO;
using System.Xml.Serialization;
using ChatterChirper.Utils;

namespace ChatterChirper.Systems
{
    public static class ConfigManager
    {
        private const string ConfigFileName = "ChatterChirperConfig.xml";
        
        public static void Load()
        {
            try 
            {
               if (File.Exists(ConfigFileName)) {
                  XmlSerializer serializer = new XmlSerializer(typeof(ModConfig));
                  using (TextReader reader = new StreamReader(ConfigFileName))
                  {
                      ModConfig.Instance = (ModConfig)serializer.Deserialize(reader);
                  }
               }
            } 
            catch { ModLogger.Error("Failed to load config"); }
        }
        
        public static void Save()
        {
             try 
             {
                 XmlSerializer serializer = new XmlSerializer(typeof(ModConfig));
                 using (TextWriter writer = new StreamWriter(ConfigFileName))
                 {
                     serializer.Serialize(writer, ModConfig.Instance);
                 }
             }
             catch { ModLogger.Error("Failed to save config"); }
        }
    }
}