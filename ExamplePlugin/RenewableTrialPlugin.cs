using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RenewableTrial
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class RenewableTrialPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Slendykray";
        public const string PluginName = "RenewableTrial";
        public const string PluginVersion = "1.0.0";


        public static DLC4TrialSelector selector;
        public static ConfigEntry<float> maxTime;
        public static ConfigEntry<float> timeAdded;

        public void Awake()
        {
            Log.Init(Logger);

            Asset.Init();

            InitConfig();

            On.RoR2.KarmaManager.ModifyKarma += KarmaManager_ModifyKarma;

            On.RoR2.DLC4TrialSelector.Awake += DLC4TrialSelector_Awake;
        }

        private void InitConfig()
        {
            Sprite icon = Asset.mainBundle.LoadAsset<Sprite>("icon.png");
            ModSettingsManager.SetModIcon(icon);


            maxTime = Config.Bind<float>("RenewableTrial", "maxTime", 240f, "maximum time trials can have (in seconds)");
            timeAdded = Config.Bind<float>("RenewableTrial", "timeAdded", 30f, "time added when receiving karma (in seconds)");

            ModSettingsManager.AddOption(new SliderOption(maxTime, new SliderConfig() { min = 180f, max = 600f }));
            ModSettingsManager.AddOption(new SliderOption(timeAdded, new SliderConfig() { min = 10f, max = 120f }));
        }


        private void DLC4TrialSelector_Awake(On.RoR2.DLC4TrialSelector.orig_Awake orig, DLC4TrialSelector self)
        {
            orig(self);
            selector = self;
        }

        private void KarmaManager_ModifyKarma(On.RoR2.KarmaManager.orig_ModifyKarma orig, KarmaManager self, int amount)
        {
            orig(self, amount);

            float timeLeft = selector.Network_trialTimerLeft;

            float addValue = timeAdded.Value;

            float newTime = timeLeft += addValue;
            if (newTime > maxTime.Value)
            {
                newTime = maxTime.Value;
            }

            selector.SetRemainingTrialTimeServer(newTime);
        }

     
    }
}
