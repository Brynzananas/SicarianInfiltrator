using BepInEx.Configuration;
using EmotesAPI;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace SicarianInfiltrator
{
    public class ModCompatabilities
    {
        public class EmoteCompatability
        {
            public const string GUID = "com.weliveinasociety.CustomEmotesAPI";
            public static void Init()
            {
                CustomEmotesAPI.ImportArmature(Assets.SicarianInfiltratorBody, Assets.SicarianInfiltratorEmote);
                Assets.SicarianInfiltratorEmote.GetComponent<BoneMapper>().scale = 1.0f;
                CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            }
            private static void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
            {
                if (mapper.name == "SicarianInfiltratorEmote")
                {
                    ChildLocator childLocator = mapper.transform.parent.GetComponent<ChildLocator>();
                    if (childLocator)
                        if (newAnimation == "none")
                        {
                            childLocator.FindChild("Gun")?.gameObject.SetActive(true);
                            childLocator.FindChild("TaserGoad")?.gameObject.SetActive(true);
                        }
                        else
                        {
                            childLocator.FindChild("Gun")?.gameObject.SetActive(false);
                            childLocator.FindChild("TaserGoad")?.gameObject.SetActive(false);
                        }

                }
            }
        }
        public class RiskOfOptionsCompatability
        {
            public const string GUID = "com.rune580.riskofoptions";
            public static void AddConfig<T>(ConfigEntry<T> config)
            {
                if (config is ConfigEntry<float>)
                {
                    ModSettingsManager.AddOption(new FloatFieldOption(config as ConfigEntry<float>));
                }
                if (config is ConfigEntry<bool>)
                {
                    ModSettingsManager.AddOption(new CheckBoxOption(config as ConfigEntry<bool>));
                }
                if (config is ConfigEntry<int>)
                {
                    ModSettingsManager.AddOption(new IntFieldOption(config as ConfigEntry<int>));
                }
                if (config is ConfigEntry<string>)
                {
                    ModSettingsManager.AddOption(new StringInputFieldOption(config as ConfigEntry<string>));
                }
                if (typeof(T).IsEnum)
                {
                    ModSettingsManager.AddOption(new ChoiceOption(config));
                }
            }
        }
    }
}
