using System;
using System.Collections.Generic;
using System.Text;
using static SicarianInfiltrator.Main;
using static SicarianInfiltrator.Keywords;
using BepInEx.Configuration;
using RoR2.CharacterAI;

namespace SicarianInfiltrator
{
    public class FireFlechetConfig
    {
        public static ConfigEntry<float> damageCoefficient = configFile.Bind(FireFlechetName, DamageCoefficientName, 0.7f, "").RegisterConfig();
        public static ConfigEntry<float> procCoefficient = configFile.Bind(FireFlechetName, ProcCoefficientName, 1f, "").RegisterConfig();
        public static ConfigEntry<float> electrifyingTimer = configFile.Bind(FireFlechetName, "Shocking Timer", 3f, "").RegisterConfig();
        public static ConfigEntry<int> electrifyingAmount = configFile.Bind(FireFlechetName, "Shocking Amount", 25, "Amount needed to Shock the target").RegisterConfig();
        public static ConfigEntry<float> shockDuration = configFile.Bind(FireFlechetName, "Shock Duration", 1f, "").RegisterConfig();
        public static ConfigEntry<float> maxDistance = configFile.Bind(FireFlechetName, "Dart Distance", 128f, "").RegisterConfig(ApplyAiRange);
        public static ConfigEntry<float> spreadBloomValue = configFile.Bind(FireFlechetName, "Spread Bloom", 0.25f, "").RegisterConfig();
        public static ConfigEntry<float> force = configFile.Bind(FireFlechetName, "Force", 30f, "").RegisterConfig();
        public static ConfigEntry<float> baseDuration = configFile.Bind(FireFlechetName, BaseDurationName, 0.2f, "").RegisterConfig();
        public static ConfigEntry<float> fireRateIncreaseOverTime = configFile.Bind(FireFlechetName, "Fire Rate Increase Over Time Rate", 2f, "").RegisterConfig();
        public static ConfigEntry<float> horizontalSpreadIncreaseOverTime = configFile.Bind(FireFlechetName, "Horizontal Spread Increase Over Time Rate", 2f, "").RegisterConfig();
        public static ConfigEntry<float> verticalSpreadIncreaseOverTime = configFile.Bind(FireFlechetName, "Vertical Spread Increase Over Time Rate", 2f, "").RegisterConfig();
        public static ConfigEntry<float> maxIncreasedFireRate = configFile.Bind(FireFlechetName, "Max Fire Rate Increase Over Time", 15f, "").RegisterConfig();
        public static ConfigEntry<float> maxIncreasedHorizontalSpread = configFile.Bind(FireFlechetName, "Max Horizontal Spread Increase Over Time", 3f, "").RegisterConfig();
        public static ConfigEntry<float> maxIncreasedVerticalSpread = configFile.Bind(FireFlechetName, "Max Vertical Spread Increase Over Time", 1f, "").RegisterConfig();
        public static ConfigEntry<float> minSpread = configFile.Bind(FireFlechetName, "Min Spread", 3f, "").RegisterConfig();
        public static ConfigEntry<int> bulletCount = configFile.Bind(FireFlechetName, "Bullet Count", 1, "").RegisterConfig();
        public static void ApplyAiRange(ConfigEntry<float> configEntry)
        {
            //if (configEntry == null) return;
            AISkillDriver aISkillDriver = Assets.AISkillDrivers[3];
            aISkillDriver.maxDistance = configEntry.Value / 2f;
            configEntry.SettingChanged += ConfigEntry_SettingChanged;
            void ConfigEntry_SettingChanged(object sender, EventArgs e)
            {
                AISkillDriver aISkillDriver = Assets.AISkillDrivers[3];
                aISkillDriver.maxDistance = configEntry.Value / 2f;
            }
        }
    }
    public class TaserGoadConfig
    {
        public static ConfigEntry<float> firstSwingDamageCoefficient = configFile.Bind(SwingTaserName, "First Swing " + DamageCoefficientName, 2f, "").RegisterConfig();
        public static ConfigEntry<float> firstSwingProcCoefficient = configFile.Bind(SwingTaserName, "First Swing " + ProcCoefficientName, 1f, "").RegisterConfig();
        public static ConfigEntry<float> firstSwingBaseDuration = configFile.Bind(SwingTaserName, "First Swing " + BaseDurationName, 0.2f, "").RegisterConfig();
        public static ConfigEntry<float> secondSwingDamageCoefficient = configFile.Bind(SwingTaserName, "Second Swing " + DamageCoefficientName, 4f, "").RegisterConfig();
        public static ConfigEntry<float> secondSwingProcCoefficient = configFile.Bind(SwingTaserName, "Second Swing " + ProcCoefficientName, 1f, "").RegisterConfig();
        public static ConfigEntry<float> secondSwingBaseDuration = configFile.Bind(SwingTaserName, "Second Swing " + BaseDurationName, 0.2f, "").RegisterConfig();
        public static ConfigEntry<float> force = configFile.Bind(SwingTaserName, "Force", 80f, "").RegisterConfig();
        public static ConfigEntry<float> baseAcceleration = configFile.Bind(SwingTaserName, "Acceleration", 5f, "").RegisterConfig();
        public static ConfigEntry<float> baseCurrent = configFile.Bind(SwingTaserName, "Current Velocity", 5f, "").RegisterConfig();
        public static ConfigEntry<float> baseMaxSpeed = configFile.Bind(SwingTaserName, "Acceleration Time", 0.2f, "").RegisterConfig();
        public static ConfigEntry<float> baseInvincibilityWindow = configFile.Bind(SwingTaserName, "Invincibility Window", 0.2f, "").RegisterConfig();
        public static ConfigEntry<bool> addLightArmor = configFile.Bind(SwingTaserName, "Add Light Armor During Skill State?", true, "").RegisterConfig();
    }
    public class HelmetSlamConfig
    {
        public static ConfigEntry<float> damageCoefficient = configFile.Bind(HelmetSlamName, DamageCoefficientName, 10f, "").RegisterConfig();
        public static ConfigEntry<float> procCoefficient = configFile.Bind(HelmetSlamName, ProcCoefficientName, 1f, "").RegisterConfig();
        public static ConfigEntry<float> force = configFile.Bind(HelmetSlamName, "Force", 200f, "").RegisterConfig();
        public static ConfigEntry<float> bonusForce = configFile.Bind(HelmetSlamName, "Bonus Force", 200f, "").RegisterConfig();
        public static ConfigEntry<float> radius = configFile.Bind(HelmetSlamName, "Radius", 12f, "").RegisterConfig();
        public static ConfigEntry<float> jumpPower = configFile.Bind(HelmetSlamName, "Jump Power", 2f, "").RegisterConfig();
        public static ConfigEntry<float> gravityScale = configFile.Bind(HelmetSlamName, "Gravity Increase", 5f, "").RegisterConfig();
        public static ConfigEntry<float> walkSpeedPenalty = configFile.Bind(HelmetSlamName, "Speed Increase", 5f, "").RegisterConfig();
        public static ConfigEntry<RoR2.BlastAttack.FalloffModel> slamDamageFalloff = configFile.Bind(HelmetSlamName, "Slam Falloff", RoR2.BlastAttack.FalloffModel.Linear, "").RegisterConfig();
    }
    public class ThrowARCGrenadeConfig
    {
        public static ConfigEntry<float> damageCoefficient = configFile.Bind(ThrowARCGrenadeName, DamageCoefficientName, 10f, "").RegisterConfig();
        public static ConfigEntry<float> baseDuration = configFile.Bind(ThrowARCGrenadeName, BaseDurationName, 0.4f, "").RegisterConfig();
        public static ConfigEntry<float> baseDistance = configFile.Bind(ThrowARCGrenadeName, "Distance", 48f, "").RegisterConfig(ApplyAiRange);
        public static void ApplyAiRange(ConfigEntry<float> configEntry)
        {
            //if (configEntry == null) return;
            AISkillDriver aISkillDriver = Assets.AISkillDrivers[0];
            aISkillDriver.maxDistance = configEntry.Value;
            configEntry.SettingChanged += ConfigEntry_SettingChanged;
            void ConfigEntry_SettingChanged(object sender, EventArgs e)
            {
                AISkillDriver aISkillDriver = Assets.AISkillDrivers[0];
                aISkillDriver.maxDistance = configEntry.Value;
            }
        }
    }
    public class UntargetableConfig
    {
        public static ConfigEntry<float> shortDamageMultiplier = configFile.Bind(UntargetableName, "Short Damage Boost Damage Multiplier", 1f, "").RegisterConfig();
        public static ConfigEntry<float> shortDamageMaxMultiplier = configFile.Bind(UntargetableName, "Short Damage Boost Damage Max Multiplier", 2.5f, "").RegisterConfig();
        public static ConfigEntry<float> shortDamageDuration = configFile.Bind(UntargetableName, "Short Damage Boost Duration", 2.5f, "").RegisterConfig();
        public static ConfigEntry<float> shortDamageTotalMultiplier = configFile.Bind(UntargetableName, "Short Damage Boost Total Multiplier", 10f, "").RegisterConfig();
    }
}
