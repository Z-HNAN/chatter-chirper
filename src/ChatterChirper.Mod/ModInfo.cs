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
            ModLogger.Info("Mod Enabled | Log file: " + ModLogger.LogFilePath);
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

            group.AddSlider("Replacement Chance [0-100]", 0, 100, 1, ModConfig.Instance.ReplaceChance, (value) =>
            {
                ModConfig.Instance.ReplaceChance = (int)value;
                ConfigManager.Save();
            });
        }
    }
}
