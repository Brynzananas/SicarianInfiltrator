using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SicarianInfiltrator.Utils;
using static SicarianInfiltrator.Keywords;
using static SicarianInfiltrator.Language;
using RoR2.CharacterAI;


namespace SicarianInfiltrator
{
    public class Assets
    {
        public static AssetBundle assetBundle;
        public static GameObject SicarianInfiltratorBody;
        public static GameObject ToolBotBody;
        public static GameObject LoaderBody;
        public static GameObject CommandoBody;
        public static GameObject SicarianInfiltratorMaster;
        public static AISkillDriver[] AISkillDrivers;
        public static CharacterBody SicarianInfiltratorBodyComponent;
        public static SurvivorDef SicarianInfiltratorSurvivor;
        public static SkinDef DefaultSkin;
        public static GameObject SicarianInfiltratorEmote;
        public static GameObject TaserSlash;
        public static GameObject Slam;
        public static GameObject LoaderSlam;
        public static GameObject Explosion;
        public static Material RoboballMaterial;
        public static GameObject Indicator;
        public static GameObject ARCGrenadeProjectile;
        public static SkillDef Untargetable;
        public static SkillDef FireFlechet;
        public static SkillDef TaserGoad;
        public static SkillDef HelmetSlam;
        public static SkillDef ThrowARCGrenade;
        public static SkillFamily Passive;
        public static SkillFamily Primary;
        public static SkillFamily Secondary;
        public static SkillFamily Utility;
        public static SkillFamily Special;
        public static BuffDef Electrifying;
        public static BuffDef ShortDamage;
        public static DamageAPI.ModdedDamageType ShockingDamageType = DamageAPI.ReserveDamageType();
        public static Dictionary<string, string> actualNames = new Dictionary<string, string>();
        public static void Init()
        {
            assetBundle = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Main.PluginInfo.Location), "assetbundles", "sicarianinfiltratorassets")).assetBundle;
            foreach (Material material in assetBundle.LoadAllAssets<Material>())
            {
                if (!material.shader.name.StartsWith("StubbedRoR2"))
                {
                    continue;
                }
                string shaderName = material.shader.name.Replace("StubbedRoR2", "RoR2") + ".shader";
                Shader replacementShader = Addressables.LoadAssetAsync<Shader>(shaderName).WaitForCompletion();
                if (replacementShader)
                {
                    material.shader = replacementShader;
                }
            }
            ToolBotBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotBody.prefab").WaitForCompletion();
            LoaderBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderBody.prefab").WaitForCompletion();
            CommandoBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion();
            SicarianInfiltratorBody = assetBundle.LoadAsset<GameObject>("Assets/SicarianInfiltrator/Character/SicarianInfiltratorBody.prefab").RegisterCharacterBody();
            SicarianInfiltratorMaster = assetBundle.LoadAsset<GameObject>("Assets/SicarianInfiltrator/Character/SicarianInfiltratorMonsterMaster.prefab").RegisterCharacterMaster();
            AISkillDrivers = SicarianInfiltratorMaster.GetComponents<AISkillDriver>();
            SicarianInfiltratorSurvivor = assetBundle.LoadAsset<SurvivorDef>("Assets/SicarianInfiltrator/Character/SicarianInfiltrator.asset").RegisterSurvivor();
            SicarianInfiltratorBodyComponent = SicarianInfiltratorBody.GetComponent<CharacterBody>();
            SicarianInfiltratorBodyComponent.preferredPodPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod");//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPod.prefab").WaitForCompletion();
            SicarianInfiltratorBodyComponent._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            SicarianInfiltratorBody.CopyComponent(ToolBotBody.GetComponent<AkBank>());
            SicarianInfiltratorBody.CopyComponent(CommandoBody.GetComponent<AkBank>());
            SicarianInfiltratorBody.CopyComponent(LoaderBody.GetComponent<AkBank>());
            GameObject gameObject = SicarianInfiltratorBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            gameObject.GetComponent<FootstepHandler>().footstepDustPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/GenericFootstepDust");
            SicarianInfiltratorEmote = assetBundle.LoadAsset<GameObject>("Assets/SicarianInfiltrator/Character/SicarianInfiltratorEmote.prefab");
            if (Main.emotesEnabled) ModCompatabilities.EmoteCompatability.Init();
            DefaultSkin = assetBundle.LoadAsset<SkinDef>("Assets/SicarianInfiltrator/Character/SicarianInfiltratorDefaultSkin.asset");
            TaserSlash = assetBundle.LoadAsset<GameObject>("Assets/SicarianInfiltrator/Effects/SwingEffect.prefab");
            Slam = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/BeetleGuardGroundSlam.prefab").WaitForCompletion();
            LoaderSlam = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderGroundSlam.prefab").WaitForCompletion();
            Explosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/OmniExplosionVFXToolbotQuick.prefab").WaitForCompletion();
            Indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
            RoboballMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/RoboBallBoss/matSummonRoboBall.mat").WaitForCompletion();
            SicarianInfiltratorSurvivor.displayPrefab.GetComponentInChildren<TemporaryOverlay>().originalMaterial = RoboballMaterial;
            Untargetable = assetBundle.LoadAsset<SkillDef>("Assets/SicarianInfiltrator/Skills/UntargetablePassive.asset").RegisterSkillDef(UntargetableName);
            FireFlechet = assetBundle.LoadAsset<SkillDef>("Assets/SicarianInfiltrator/Skills/FireFlechet.asset").RegisterSkillDef(FireFlechetName);
            TaserGoad = assetBundle.LoadAsset<SkillDef>("Assets/SicarianInfiltrator/Skills/TaserGoad.asset").RegisterSkillDef(SwingTaserName);
            HelmetSlam = assetBundle.LoadAsset<SkillDef>("Assets/SicarianInfiltrator/Skills/HelmetSlam.asset").RegisterSkillDef(HelmetSlamName);
            ThrowARCGrenade = assetBundle.LoadAsset<SkillDef>("Assets/SicarianInfiltrator/Skills/ThrowARCGrenade.asset").RegisterSkillDef(ThrowARCGrenadeName);
            Passive = assetBundle.LoadAsset<SkillFamily>("Assets/SicarianInfiltrator/SkillFamilies/SicarianInfiltratorPassive.asset").RegisterSkillFamily();
            Primary = assetBundle.LoadAsset<SkillFamily>("Assets/SicarianInfiltrator/SkillFamilies/SicarianInfiltratorPrimary.asset").RegisterSkillFamily();
            Secondary = assetBundle.LoadAsset<SkillFamily>("Assets/SicarianInfiltrator/SkillFamilies/SicarianInfiltratorSecondary.asset").RegisterSkillFamily();
            Utility = assetBundle.LoadAsset<SkillFamily>("Assets/SicarianInfiltrator/SkillFamilies/SicarianInfiltratorUtillity.asset").RegisterSkillFamily();
            Special = assetBundle.LoadAsset<SkillFamily>("Assets/SicarianInfiltrator/SkillFamilies/SicarianInfiltratorSpecial.asset").RegisterSkillFamily();
            Electrifying = assetBundle.LoadAsset<BuffDef>("Assets/SicarianInfiltrator/Buffs/Electrifying.asset").RegisterBuffDef();
            ShortDamage = assetBundle.LoadAsset<BuffDef>("Assets/SicarianInfiltrator/Buffs/ShortDamage.asset").RegisterBuffDef();
            ARCGrenadeProjectile = assetBundle.LoadAsset<GameObject>("Assets/SicarianInfiltrator/Projectiles/ARCGrenade.prefab").RegisterProjectile(ThrowARCGrenadeName);
        }
    }
}
