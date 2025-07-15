using System.Security.Permissions;
using System.Security;
using BepInEx;
using R2API;
using R2API.Utils;
using RoR2.ContentManagement;
using BepInEx.Configuration;
using RoR2.UI;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: HG.Reflection.SearchableAttribute.OptIn]
[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
[module: UnverifiableCode]
#pragma warning disable CS0618
#pragma warning restore CS0618
namespace SicarianInfiltrator
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(R2API.SkillsAPI.PluginGUID, SkillsAPI.PluginVersion)]
    [BepInDependency(R2API.SkillsAPI.PluginGUID, SkillsAPI.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    //[R2APISubmoduleDependency(nameof(CommandHelper))]
    [System.Serializable]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.brynzananas.sicarianinfiltrator";
        public const string ModName = "Sicarian Infiltrator";
        public const string ModVer = "1.0.0";
        public static bool emotesEnabled;
        public static bool riskOfOptionsEnabled;
        public static PluginInfo PluginInfo { get; private set; }
        public static ConfigFile configFile { get; private set; }
        
        public void Awake()
        {
            PluginInfo = Info;
            configFile = Config;
            emotesEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ModCompatabilities.EmoteCompatability.GUID);
            riskOfOptionsEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ModCompatabilities.RiskOfOptionsCompatability.GUID);
            Assets.Init();
            Hooks.SetHooks();
            new FireFlechetConfig();
            new TaserGoadConfig();
            new HelmetSlamConfig();
            new ThrowARCGrenadeConfig();
            new UntargetableConfig();
            ContentManager.collectContentPackProviders += (addContentPackProvider) =>
            {
                addContentPackProvider(new ContentPacks());
            };
        }
    }
}
