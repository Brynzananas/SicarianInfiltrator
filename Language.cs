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
        public static string UntargetableDesc => $"While out of combat, turn {styleUtility}invisible{styleExit} and receive a small boost to {styleUtility}movement speed{styleExit} and {styleDamage}damage{styleExit}. {styleDamage}Damage boost{styleExit} persists after entering combat and lingers down.";
        public static string FireFlechetDesc => $"{styleDamage}Electrifying{styleExit}. Rapidly shoot an enemy for {styleDamage + FireFlechetConfig.damageCoefficient.Value * 100}% damage{styleExit}. Fire rate and horizontal spread increases over time.";
        public static string TaserGoadDesc => $"{styleDamage}Shocking{styleExit}. Swing in front on enter for {styleDamage + TaserGoadConfig.firstSwingDamageCoefficient.Value * 100}% damage{styleExit} and swing again on button release for {styleDamage + TaserGoadConfig.secondSwingDamageCoefficient.Value * 100}% damage{styleExit}.";
        public static string HelmetSlamDesc => $"{styleDamage}Stunning{styleExit}. Jump up and slam down, dealing {styleDamage + HelmetSlamConfig.damageCoefficient.Value * 100}% damage{styleExit} on impact. Movement speed is increased by {styleUtility + HelmetSlamConfig.walkSpeedPenalty.Value * 100}%{styleExit} during slam attack.";
        public static string ThrowARCGrenadeDesc => $"{styleDamage}Stunning{styleExit}. Throw a grenade that explodes for {styleDamage + ThrowARCGrenadeConfig.damageCoefficient.Value * 100}% damage{styleExit}.";
        public static string ElectrifyingDesc => $"{styleKeyword}Electrifying{styleExit}{styleSub}Hittting an enemy {FireFlechetConfig.electrifyingAmount.Value} times will apply {styleDamage}Shock{styleExit} for {FireFlechetConfig.shockDuration.Value} seconds{styleExit}";
        public static string UntargetableDescRu => $"Вне опасности, обретите {styleUtility}невидимость{styleExit} и получите малый бонус к {styleUtility}скорости передвижения{styleExit} и {styleDamage}урону{styleExit}. {styleDamage}Бонус к урону{styleExit} остается после вхождения в бой, но бастро угасает.";
        public static string FireFlechetDescRu => $"{styleDamage}Повышение Тока{styleExit}. Быстро стреляет во врага и наносит {styleDamage + FireFlechetConfig.damageCoefficient.Value * 100}% урона{styleExit}. Скорость атаки и горизонтальный разброс повышается со временем.";
        public static string TaserGoadDescRu => $"{styleDamage}Разряд{styleExit}. Взмахни вперёд нанося {styleDamage + TaserGoadConfig.firstSwingDamageCoefficient.Value * 100}% урона{styleExit} и взмахни снова при отпускании кнопки нанося {styleDamage + TaserGoadConfig.secondSwingDamageCoefficient.Value * 100}% урона{styleExit}.";
        public static string HelmetSlamDescRu => $"{styleDamage}Оглушение{styleExit}. Прыгни и взерни вниз, нанося {styleDamage + HelmetSlamConfig.damageCoefficient.Value * 100}% урона{styleExit} при приземлении. Скорость передвижения увеличена в {styleUtility + HelmetSlamConfig.walkSpeedPenalty.Value * 100}%{styleExit} во время падения.";
        public static string ThrowARCGrenadeDescRu => $"{styleDamage}Оглушение{styleExit}. После броска эта граната взрывается и наносит {styleDamage + ThrowARCGrenadeConfig.damageCoefficient.Value * 100}% урона{styleExit}.";
        public static string ElectrifyingDescRu => $"{styleKeyword}Повышение Тока{{{styleExit}{styleSub}Ударив по врагу {FireFlechetConfig.electrifyingAmount.Value} раз враг {styleDamage}Разрядится{styleExit} на {FireFlechetConfig.shockDuration.Value} секунд{styleExit}";
        public static string ElectrifyingName => "KEYWORD_ELECTRIFYING";
        public static void Init()
        {
            AddLanguageToken(Assets.Untargetable.skillNameToken, UntargetableName);
            AddLanguageToken(Assets.Untargetable.skillNameToken, UntargetableNameRu, "ru");
            AddLanguageToken(Assets.Untargetable.skillDescriptionToken, UntargetableDesc);
            AddLanguageToken(Assets.Untargetable.skillDescriptionToken, UntargetableDescRu, "ru");
            AddLanguageToken(Assets.FireFlechet.skillNameToken, FireFlechetName);
            AddLanguageToken(Assets.FireFlechet.skillNameToken, FireFlechetNameRu, "ru");
            AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDesc);
            AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDescRu, "ru");
            AddLanguageToken(ElectrifyingName, ElectrifyingDesc);
            AddLanguageToken(ElectrifyingName, ElectrifyingDescRu, "ru");
            FireFlechetConfig.damageCoefficient.SettingChanged += FireFlechetChangeSec;
            void FireFlechetChangeSec(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDesc);
                AddLanguageToken(Assets.FireFlechet.skillDescriptionToken, FireFlechetDescRu, "ru");
            }
            void ShockingAmount_SettingChanged(object sender, EventArgs e)
            {
                AddLanguageToken(ElectrifyingName, ElectrifyingDesc);
                AddLanguageToken(ElectrifyingName, ElectrifyingDescRu, "ru");
            }
            FireFlechetConfig.electrifyingAmount.SettingChanged += ShockingAmount_SettingChanged;
            FireFlechetConfig.shockDuration.SettingChanged += ShockingAmount_SettingChanged;
            AddLanguageToken(Assets.TaserGoad.skillNameToken, SwingTaserName);
            AddLanguageToken(Assets.TaserGoad.skillNameToken, SwingTaserNameRu, "ru");
            AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDesc);
            AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDescRu, "ru");
            TaserGoadConfig.firstSwingDamageCoefficient.SettingChanged += TaserGoadChangeDesc;
            TaserGoadConfig.secondSwingDamageCoefficient.SettingChanged += TaserGoadChangeDesc;
            void TaserGoadChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDesc);
                AddLanguageToken(Assets.TaserGoad.skillDescriptionToken, TaserGoadDescRu, "ru");
            }
            AddLanguageToken(Assets.HelmetSlam.skillNameToken, HelmetSlamName);
            AddLanguageToken(Assets.HelmetSlam.skillNameToken, HelmetSlamNameRu, "Ru");
            AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDesc);
            AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDescRu, "ru");
            HelmetSlamConfig.damageCoefficient.SettingChanged += HelmetSlamChangeDesc;
            HelmetSlamConfig.walkSpeedPenalty.SettingChanged += HelmetSlamChangeDesc;
            void HelmetSlamChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDesc);
                AddLanguageToken(Assets.HelmetSlam.skillDescriptionToken, HelmetSlamDescRu, "ru");
            }
            AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeName);
            AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeNameRu, "ru");
            AddLanguageToken(Assets.ThrowARCGrenade.skillDescriptionToken, ThrowARCGrenadeDesc);
            AddLanguageToken(Assets.ThrowARCGrenade.skillDescriptionToken, ThrowARCGrenadeDescRu, "ru");
            ThrowARCGrenadeConfig.damageCoefficient.SettingChanged += ThrowARCGrenadeChangeDesc;
            void ThrowARCGrenadeChangeDesc(object sender, EventArgs e)
            {
                AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeDesc);
                AddLanguageToken(Assets.ThrowARCGrenade.skillNameToken, ThrowARCGrenadeDescRu, "ru");
            }
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken, BodyName);
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken, BodyNameRu, "ru");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.subtitleNameToken, "Sinister Warrior");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.subtitleNameToken, "Мрачный Воин", "ru");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.descriptionToken, "" +
                "Sicarian Infiltrator is a stealthy and swift character that is reliable in all situations of the game. <style=cSub>\r\n\r\n< ! > Untargetable makes you invisible while out of combat and provides a small boost to movement speed and damage. Use it to ambush enemies!.\r\n\r\n< ! > Flechet is powerful at both short and long range, firing quickly, shocking enemies and activating many item effects.\r\n\r\n< ! > Dome Helmet Slam is both an excelent ability for fast movement and daamaging a group of enemies.\r\n\r\n< ! > ARC Grenade can be used to heavily damage and stun a group of enemies.\r\n\r\n< ! > Taser Goad slashes through enemy groups and shocks them, while giving short invincibility duration and small armor boost, use it to your advantage!\r\n</style>\r\n" +
                "");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.descriptionToken, "" +
                "Сикариан Десантник — быстрый и скрытный персонаж. На него можно положиться в любой игровой ситуации. <style=cSub>\r\n\r\n< ! > Нецелевой делает обладателя невидимым вне опасности и дает небольшой бонус к скорости передвижению и урону.\r\n\r\n< ! > Флешет хорош и на коротких, и на длинных дистанциях. Этот быстрый навык, который разряжает врагов и активирует множество эффектов от предметов.\r\n\r\n< ! > Грохот купольным шлемом это отличный навык для передвижения и нанесения урона по группе врагов.\r\n\r\n< ! > ARC Граната может быть использована чтобы наносить тяжелый урона и останавливать группу врагов.\r\n\r\n< ! > Удар Электрошокером разрезает врагов и разряжает их, выдавая атакующему неуязвимость на короткое время и маленбкий бонус к броне на протяжении всего приема!\r\n</style>\r\n" +
                "", "ru");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.displayNameToken, BodyName);
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.displayNameToken, BodyNameRu, "ru");
            AddLanguageToken(Assets.SicarianInfiltratorSurvivor.displayNameToken, BodyName);
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken.Replace("_NAME", "_OUTRO"), "..and so he left, as he found a way to return to his Adeptus Master.");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken.Replace("_NAME", "_OUTRO"), "...и он ушел, найдя путь вернуться к его Адептусу Мастеру.", "ru");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken.Replace("_NAME", "_FAIL"), "..and so he vanished, as he couldn't manage to return to his Adeptus Master.");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken.Replace("_NAME", "_FAIL"), "...и он исчез, не найдя путь вернуться к его Адептусу Мастеру.", "ru");
            AddLanguageToken(Assets.SicarianInfiltratorBodyComponent.baseNameToken.Replace("_NAME", "_LORE"), "" +
                "A Sicarian Infiltrator is a member of perhaps the most sinister of the Adeptus Mechanicus' Skitarii warrior clades, for their neurostatic bombardment robs their victims of their senses. When hunting, they emit a white noise that fills the visual, auditory and even olfactory spectrums with static, leaving their foes all but helpless before the killing begins.\r\n\r\nTall and slender, Sicarian Infiltrators pick their way across the battlefield with the stilted grace of spearfisher birds. They were not always this way, however, for each Sicarian is far from whole -- not in spirit, nor in body.\r\n\r\nAs with their Ruststalker brethren, every Sicarian Infiltrator was once a warrior of the Skitarii who, in the pursuance of the Cult Mechanicus' agenda, was blasted limb from limb, extensively burned or otherwise dismembered. During the data-harvest at a battle's end, if these fallen are judged still fit to serve the Omnissiah, they are not incinerated but instead taken back to the augmetic slabs.\r\n\r\nThere they are given a new lease on life by the addition of slender but powerful metal limbs. Technically speaking, all the Magi Biologis need to create a Sicarian is a head, a torso and some limb stumps, though a detachment from emotion and a knack for inspiring fear is vital for the best results.\r\n\r\nWith such awesome power at their command, it is tempting for Sicarian Infiltrators to employ their neurostatic off the battlefield, against uncooperative Imperial citizens. This escalation often leads to unnecessary complications, and Infiltrators must struggle to reconnect with their dwindling shreds of Humanity and discover less direct ways to achieve their aims.\r\n\r\n\r\nContents\r\n1\tRole\r\n1.1\tNotable Formations\r\n2\tUnit Composition\r\n3\tWargear\r\n4\tSources\r\n5\tGallery\r\nRole\r\nSicarian Infiltrator\r\nThis Sicarian Infiltrator stands ready to face the foes of the Machine God.\r\n\r\nAn Infiltrator can bypass enemy defences with ease, though this ability owes nothing to stealth, nor skill. Instead it hinges upon the potency and variety of the disruptive wavelengths they broadcast from their domed helms and jutting antennae.\r\n\r\nWhen the Infiltrators approach, their prey's every sensory apparatus is bombarded by overwhelming stimuli. Vox-casters howl with anguished feedback whilst vid-screens craze with fizzing static. Yet this crippling electromagnetic assault is even more effective upon natural senses than artificial ones.\r\n\r\nEars ring with cruel tinnitus, eyes water and turn red, and the taste of burnt metal fills the mouth. It is all the Infiltrators' victims can do to remember how to breathe. As their prey clasps hands over bleeding ears and screws shut bloodshot eyes, the Sicarian Infiltrators simply walk into point-blank range and open fire.\r\n\r\nThis sensory assault, though broad in spectrum, is calibrated precisely by the tech-priest sanctioning these macabre assassins. Those Skitarii sent to fight alongside the Infiltrators are given null codes that transmute the frequencies into harmless song; to them, the constant barrarge of neurostatic coming from each domed helmet is nothing more than a soft psalm to the Omnissiah's glory.\r\n\r\nFor this reason Infitrators are seen as wise and holy heroes by their Skitarii brethren, talismans against data corruption that fight a selfless war on the front line. Only to the enemy is truth revealed. There is little virtue left inside these merciless prowlers, and what personality remains is interested only in death.\r\n\r\nNotable Formations\r\nSicarian Killclade - The vile hissing that accompanies a Sicarian Killclade on the hunt gnaws at the mind. When the stomach-churning hum of transonic weaponry mingles with the mind numbing aura of an Infiltrator assault, its effect can be magnified, leaving those brave or foolish enough to stand their ground all but crippled. The sight of their foes reeling from their approach fills the Sicarians with righteous faith, readying them for the hyperaction imperatives their masters inload as they near the foe. At an unspoken command, the Sicarian Ruststalkers of the Killclade will burst from concealment like hunter-arachnids. Their Transonic Weapons flash azure as they plunge headlong into the foe with reckless haste, then gory red as the butchery begins. A Sicarian Killclade typically consists of three squads of Sicarian Ruststalkers accompanied by a single squad of Sicarian Infiltrators.\r\nUnit Composition\r\n4-9 Sicarian Infiltrators\r\n1 Infiltrator Princeps\r\nWargear\r\nAs standard, all Sicarian Infiltrators are armed with:\r\n\r\nSicarian Battle Armour - As agility is of paramount importance to the long-limbed killers of the Sicarian brotherhood, Sicarian Infiltrators go to war clad in Sicarian Battle Armour. This is made up of a multi-layered alloy that, though thin and flexible, provides admirable physical protection. This alloy, informally known as aegium, acts as a capacitor that harnesses the energy of incoming attacks and disperses it harmlessly across the wearers bionic frame.\r\nStubcarbine - A Stubcarbine, though compact, has the stopping power of the Heavy Stubbers mounted on the tanks of the Astra Militarum. When a squad of Sicarians opens fire with these weapons, the air fills with a storm of solid shot that chews their victims to ruin.\r\nPower Sword - A Power Weapon is sheathed in the lethal haze of a disruptive energy field that eats through armour, flesh and bone with ease.\r\nAll members of a Sicarian Infiltrator squad can choose to replace their Stubcarbine and Power Sword with:\r\n\r\nFlechette Blaster - A Flechette Blaster is a lightweight but lethal weapon, a favoured tool of Sicarian Infiltrators. It fires hundreds of tiny darts, each of which bears a dormant cerebral cell awakened in the gun's chamber. Where one dart hits home, it emits a bioelectric pulse that attracts others, resulting in a series of impacts that burrow through bone.\r\nTaser Goad - Powered by hyperdynamo capacitors, Taser Weapons store an incredible amount of potential energy. A solid impact will cause this energy to be discharged in a scorching blast, only to be harnessed once more by the electrothief prongs at the weapon's tip.\r\nOptional wargear that an Infiltrator Princeps can take include:\r\n\r\nInfoslave Skull - The dextrous skull-and-digit adjutants that accompany senior Skitarii record hard data at a prolific rate. To know their findings are recorded and reported back is a great boon to the Skitarii, who draw courage and strength from the fact their sacrifices will not be in vain.\r\nRelics of Mars - Relics of Mars are items of terrifying power that are sometimes bestowed upon a Skitarii Alpha or Sicarian Princeps by a senior tech-priest for them to field test.\r\nSpecial Issue Wargear - Elite Skitarii warriors have the right to bear special issue wargear into battle, which can include either a Conversion Field or Refractor Field, and Digital Weapons.\r\nSources\r\nCodex: Adeptus Mechanicus (8th Edition), pp. 51, 79\r\nCodex: Adeptus Mechanicus - Skitarii (7th Edition), pp. 28, 32, 34-39, 60, 66\r\nWrath & Glory: Forsaken System Player Guide (RPG), pg. 110" +
                "");
        }

        public static void AddLanguageToken(string token, string output, string language)
        {
            Debug.Log("Adding token: " + token + "\nWith output: " + output + "\nTo language: " + language);
            Dictionary<string, string> keyValuePairs = RoR2.Language.languagesByName.ContainsKey(language) ? RoR2.Language.languagesByName[language].stringsByToken : null;
            Debug.Log(keyValuePairs);
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
        public const string styleKeyword = "<style=cKeywordName>";
        public const string styleSub = "<style=cSub>";
    }
}
