using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static SicarianInfiltrator.Keywords;

namespace SicarianInfiltrator
{
    public class Language
    {
        public static string UntargetableDesc => $"{styleUtility}Turn invisible while out of combat{styleExit} and receive a small boost to speed and damage. {styleDamage}Damage boost{styleExit} persists after entering combat and lingers down.";
        public static string FireFlechetDesc => $"Rapidly shoot an enemy for {styleDamage + FireFlechetConfig.damageCoefficient.Value * 100}% damage{styleExit}. Fire rate and horizontal spread increases over time.";
        public static string TaserGoadDesc => $"{styleDamage}Shocking{styleExit}. Swing in front on enter for {styleDamage + TaserGoadConfig.firstSwingDamageCoefficient.Value * 100}% damage{styleExit} and swing again on button release for {styleDamage + TaserGoadConfig.secondSwingDamageCoefficient.Value * 100}% damage{styleExit}.";
        public static string HelmetSlamDesc => $"{styleDamage}Stunning{styleExit}. Jump up and slam down, dealing {styleDamage + HelmetSlamConfig.damageCoefficient.Value * 100}% damage{styleExit} on impact. Movement speed is increased by {styleUtility + HelmetSlamConfig.walkSpeedPenalty.Value * 100}%{styleExit} during slam attack.";
        public static string ThrowARCGrenadeDesc => $"{styleDamage}Stunning{styleExit}. Throw a grenade that explodes for {styleDamage + ThrowARCGrenadeConfig.damageCoefficient.Value * 100}% damage{styleExit}.";
        public static void Init()
        {
            AddLanguageToken(Assets.Untargetable.skillNameToken, UntargetableName);
            AddLanguageToken(Assets.Untargetable.skillDescriptionToken, UntargetableDesc);
            AddLanguageToken(Assets.FireFlechet.skillNameToken, FireFlechetName);
            AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDesc);
            FireFlechetConfig.damageCoefficient.SettingChanged += FireFlechetChangeSec;
            void FireFlechetChangeSec(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDesc);
            }
            AddLanguageToken(Assets.TaserGoad.skillNameToken, SwingTaserName);
            AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDesc);
            TaserGoadConfig.firstSwingDamageCoefficient.SettingChanged += TaserGoadChangeDesc;
            TaserGoadConfig.secondSwingDamageCoefficient.SettingChanged += TaserGoadChangeDesc;
            void TaserGoadChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDesc);
            }
            AddLanguageToken(Assets.HelmetSlam.skillNameToken, HelmetSlamName);
            AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDesc);
            HelmetSlamConfig.damageCoefficient.SettingChanged += HelmetSlamChangeDesc;
            HelmetSlamConfig.walkSpeedPenalty.SettingChanged += HelmetSlamChangeDesc;
            void HelmetSlamChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDesc);
            }
            AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeName);
            AddLanguageToken(Assets.ThrowARCGrenade.skillDescriptionToken, ThrowARCGrenadeDesc);
            ThrowARCGrenadeConfig.damageCoefficient.SettingChanged += ThrowARCGrenadeChangeDesc;
            void ThrowARCGrenadeChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeDesc);
            }
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken, BodyName);
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.subtitleNameToken, "Balls Hammer");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.descriptionToken, "sus");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.displayNameToken, BodyName);
        }

        public static void AddLanguageToken(string token, string output, string language)
        {
            Dictionary<string, string> keyValuePairs = RoR2.Language.languagesByName.ContainsKey(language) ? RoR2.Language.languagesByName[language].stringsByToken : null;
            if (keyValuePairs == null) return;
            if (keyValuePairs.ContainsKey(token))
            {
                keyValuePairs[token] = output;
            }
            else
            {
                keyValuePairs.Add(token, output);
            }
        }
        public static void AddLanguageToken(string token, string output)
        {
            AddLanguageToken(token, output, "en");
        }
        public const string styleExit = "</style>";
        public const string styleDamage = "<style=cIsDamage>";
        public const string styleUtility = "<style=cIsUtility>";
    }
}
