using BepInEx.Configuration;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace SicarianInfiltrator
{
    public class Utils
    {
        public class SkillDefCreator
        {
            public Type state;
            public string activationState;
            public Sprite sprite;
            public string name;
            public string nameToken;
            public string descToken;
            public string[] keywordsTokens;
            public int maxStocks = 1;
            public float rechargeInterval;
            public bool beginSkillCooldownOnSkillEnd = true;
            public bool canceledFromSprinting;
            public bool cancelSprinting = true;
            public bool fullRestockOnAssign = true;
            public InterruptPriority interruptPriority = InterruptPriority.Any;
            public bool isCombat;
            public bool mustKeyPress;
            public int requiredStock;
            public int rechargeStock;
            public int stockToConsume;
            public SkillFamily skillFamily;
            public bool resetCooldownTimerOnUse;
        }
        public static T AddSkill<T>(SkillDefCreator skillDefCreator) where T : SkillDef
        {
            T mySkillDef = AddSkill<T>(skillDefCreator.state, skillDefCreator.activationState, skillDefCreator.sprite, skillDefCreator.name, skillDefCreator.nameToken, skillDefCreator.descToken, skillDefCreator.keywordsTokens, skillDefCreator.maxStocks, skillDefCreator.rechargeInterval, skillDefCreator.beginSkillCooldownOnSkillEnd, skillDefCreator.canceledFromSprinting, skillDefCreator.cancelSprinting, skillDefCreator.fullRestockOnAssign, skillDefCreator.interruptPriority, skillDefCreator.isCombat, skillDefCreator.mustKeyPress, skillDefCreator.requiredStock, skillDefCreator.rechargeStock, skillDefCreator.stockToConsume, skillDefCreator.skillFamily, skillDefCreator.beginSkillCooldownOnSkillEnd);
            return mySkillDef;
        }
        public static T AddSkill<T>(Type state, string activationState, Sprite sprite, string name, string nameToken, string descToken, string[] keywordTokens, int maxStocks, float rechargeInterval, bool beginSkillCooldownOnSkillEnd, bool canceledFromSprinting, bool cancelSprinting, bool fullRestockOnAssign, InterruptPriority interruptPriority, bool isCombat, bool mustKeyPress, int requiredStock, int rechargeStock, int stockToConsume, SkillFamily skillFamily, bool resetCooldownTimerOnUse = false) where T : SkillDef
        {
            GameObject commandoBodyPrefab = Assets.SicarianInfiltratorBody;

            T mySkillDef = ScriptableObject.CreateInstance<T>();
            mySkillDef.SetBonusStockMultiplier(maxStocks);
            //skillsBonusStocksMultiplier.Add(mySkillDef, maxStocks);
            mySkillDef.activationState = new SerializableEntityStateType(state);
            mySkillDef.activationStateMachineName = activationState;
            mySkillDef.baseMaxStock = maxStocks;
            mySkillDef.baseRechargeInterval = rechargeInterval;
            mySkillDef.beginSkillCooldownOnSkillEnd = beginSkillCooldownOnSkillEnd;
            mySkillDef.canceledFromSprinting = canceledFromSprinting;
            mySkillDef.cancelSprintingOnActivation = cancelSprinting;
            mySkillDef.fullRestockOnAssign = fullRestockOnAssign;
            mySkillDef.interruptPriority = interruptPriority;
            mySkillDef.isCombatSkill = isCombat;
            mySkillDef.mustKeyPress = mustKeyPress;
            mySkillDef.rechargeStock = rechargeStock;
            mySkillDef.requiredStock = requiredStock;
            mySkillDef.stockToConsume = stockToConsume;
            mySkillDef.icon = sprite;
            mySkillDef.skillDescriptionToken = descToken;
            mySkillDef.skillName = nameToken;
            mySkillDef.skillNameToken = nameToken;
            mySkillDef.keywordTokens = keywordTokens;
            mySkillDef.resetCooldownTimerOnUse = resetCooldownTimerOnUse;
            (mySkillDef as ScriptableObject).name = name;
            ContentPacks.skills.Add(mySkillDef);
            if (skillFamily)
                AddSkillToFamily(ref skillFamily, mySkillDef);
            return mySkillDef as T;
        }
        public static void AddSkillToFamily(ref SkillFamily skillFamily, SkillDef skillDef)
        {
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }
    }
    public static class Keywords
    {
        public const string BodyName = "Sicarian Infiltrator";
        public const string UntargetableName = "Untargetable";
        public const string FireFlechetName = "Flechet";
        public const string SwingTaserName = "Taser Goad";
        public const string HelmetSlamName = "Dome Helmet Slam";
        public const string ThrowARCGrenadeName = "ARC Grenade";
        public const string DamageCoefficientName = "Damage Coefficient";
        public const string ProcCoefficientName = "Proc Coefficient";
        public const string BaseDurationName = "Base Duration";
    }
    public static class Extensions
    {
        public static T RegisterSkillDef<T>(this T skillDef, string name) where T : SkillDef
        {
            Assets.actualNames.Add(skillDef.skillName, name);
            if (name != Keywords.UntargetableName)
            {
                ConfigEntry<int> maxStock = Main.configFile.Bind(name, "Base Max Stock", skillDef.baseMaxStock, "").RegisterConfig();
                skillDef.baseMaxStock = maxStock.Value;
                maxStock.SettingChanged += ChangeMaxStock;
                void ChangeMaxStock(object sender, EventArgs e)
                {
                    skillDef.baseMaxStock = maxStock.Value;
                }
                ConfigEntry<int> rechargeStock = Main.configFile.Bind(name, "Base Recharge Stock", skillDef.rechargeStock, "").RegisterConfig();
                skillDef.rechargeStock = rechargeStock.Value;
                rechargeStock.SettingChanged += ChangeRechargeStock;
                void ChangeRechargeStock(object sender, EventArgs e)
                {
                    skillDef.rechargeStock = rechargeStock.Value;
                }
                ConfigEntry<float> recharge = Main.configFile.Bind(name, "Base Recharge Interval", skillDef.baseRechargeInterval, "").RegisterConfig();
                skillDef.baseRechargeInterval = recharge.Value;
                recharge.SettingChanged += ChangeRecharge;
                void ChangeRecharge(object sender, EventArgs e)
                {
                    skillDef.baseRechargeInterval = recharge.Value;
                }
            }
            ContentPacks.skills.Add(skillDef);
            return skillDef;
        }

        public static T RegisterSkillFamily<T>(this T skillFamily) where T : SkillFamily
        {
            ContentPacks.skillFamilies.Add(skillFamily);
            return skillFamily;
        }
        public static GameObject RegisterCharacterBody(this GameObject body)
        {
            ContentPacks.bodies.Add(body);
            return body;
        }
        public static T RegisterSurvivor<T>(this T survivorDef) where T : SurvivorDef
        {
            ContentPacks.survivors.Add(survivorDef);
            return survivorDef;
        }
        public static T RegisterBuffDef<T>(this T buffDef) where T : BuffDef
        {
            ContentPacks.buffs.Add(buffDef);
            return buffDef;
        }
        public static GameObject RegisterProjectile(this GameObject projectile, string name)
        {
            Assets.actualNames.Add(projectile.name, name);
            string sectionName = name;
            ProjectileImpactExplosion projectileImpactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            if (projectileImpactExplosion != null)
            {
                ConfigEntry<float> blastRadius = Main.configFile.Bind(sectionName, "Blast Radius", projectileImpactExplosion.blastRadius, "").RegisterConfig();
                projectileImpactExplosion.blastRadius = blastRadius.Value;
                blastRadius.SettingChanged += ChangeRadius;
                void ChangeRadius(object sender, EventArgs e)
                {
                    projectileImpactExplosion.blastRadius = blastRadius.Value;
                }
                ConfigEntry<BlastAttack.FalloffModel> fallOffModel = Main.configFile.Bind(sectionName, "Explosion Damage Falloff", projectileImpactExplosion.falloffModel, "").RegisterConfig();
                projectileImpactExplosion.falloffModel = fallOffModel.Value;
                fallOffModel.SettingChanged += ChangeFalloff;
                void ChangeFalloff(object sender, EventArgs e)
                {
                    projectileImpactExplosion.falloffModel = fallOffModel.Value;
                }
                if (projectileImpactExplosion.explosionEffect == null)
                projectileImpactExplosion.explosionEffect = Assets.Explosion;
            }
            ProjectileSimple projectileSimple = projectile.GetComponent<ProjectileSimple>();
            if (projectileSimple != null)
            {
                ConfigEntry<float> startSpeed = Main.configFile.Bind(sectionName, "Start Speed", projectileSimple.desiredForwardSpeed, "").RegisterConfig();
                projectileSimple.desiredForwardSpeed = startSpeed.Value;
                startSpeed.SettingChanged += ChangeRecharge;
                void ChangeRecharge(object sender, EventArgs e)
                {
                    projectileSimple.desiredForwardSpeed = startSpeed.Value;
                }
            }
            ContentPacks.projectiles.Add(projectile);
            ContentPacks.networkPrefabs.Add(projectile);
            return projectile;
        }
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
        public static ConfigEntry<T> RegisterConfig<T>(this ConfigEntry<T> configEntry)
        {
            if(Main.riskOfOptionsEnabled)
            ModCompatabilities.RiskOfOptionsCompatability.AddConfig(configEntry);
            return configEntry;
        }
    }
}
