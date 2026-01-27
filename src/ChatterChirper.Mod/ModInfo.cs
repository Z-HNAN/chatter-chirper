using ICities;
using ChatterChirper.Utils;
using ChatterChirper.Systems;

namespace ChatterChirper
{
    public class ModInfo : IUserMod
    {
        public string Name => $"{Constants.ModName} v{Constants.Version}";
        public string Description => Constants.ModDescription;

        public void OnEnabled()
        {
            ConfigManager.Load();
            ModLogger.Info("Mod Enabled");
            if (ModConfig.Instance.Enabled)
                Patcher.PatchAll();
        }

        public void OnDisabled()
        {
            Patcher.UnpatchAll();
            ModLogger.Info("Mod Disabled");
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup("General Setting");

            group.AddCheckbox("Enabled", ModConfig.Instance.Enabled, (isChecked) =>
            {
                ModConfig.Instance.Enabled = isChecked;
                ConfigManager.Save();
                if (isChecked) Patcher.PatchAll();
                else Patcher.UnpatchAll();
            });

            group.AddSlider("Toxicity Level", 0.0f, 2.0f, 0.1f, ModConfig.Instance.Toxicity, (val) =>
            {
                ModConfig.Instance.Toxicity = val;
                ConfigManager.Save();
            });
            
             group.AddSlider("Replace Probability", 0.0f, 1.0f, 0.1f, ModConfig.Instance.ReplaceProbability, (val) =>
            {
                ModConfig.Instance.ReplaceProbability = val;
                ConfigManager.Save();
            });
        }
    }
}