using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SicarianInfiltrator
{
    public class Hooks
    {
        public static void SetHooks()
        {
            On.RoR2.CharacterBody.HandleCascadingBuffs += CharacterBody_HandleCascadingBuffs;
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }
        public static float MaxShortDamageMultipler => UntargetableConfig.shortDamageMultiplier.Value;
        public static float MaxShortDamageMaxMultipler => UntargetableConfig.shortDamageMaxMultiplier.Value;
        public static float MaxShortDamageTotal => UntargetableConfig.shortDamageTotalMultiplier.Value;
        public static float ShockingTimer => FireFlechetConfig.shockingTimer.Value;
        public static float ShockDuration => FireFlechetConfig.shockingDuration.Value;
        public static int ShockAmmount => FireFlechetConfig.shockingAmount.Value;
        private static void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            if (attackerBody != null)
            {
                List<CharacterBody.TimedBuff> timedBuffs = attackerBody.timedBuffs;
                float multiplier = 0f;
                int buffCount = 0;
                for (int i = 0; i < timedBuffs.Count; i++)
                {
                    CharacterBody.TimedBuff timedBuff = timedBuffs[i];
                    if(timedBuff == null || timedBuff.buffIndex != Assets.ShortDamage.buffIndex) continue;
                    buffCount++;
                    multiplier += MathF.Min(timedBuff.timer, MaxShortDamageMaxMultipler);
                }
                multiplier += (attackerBody.GetBuffCount(Assets.ShortDamage) - buffCount) * MaxShortDamageMaxMultipler;
                damageInfo.damage *= Mathf.Max(Mathf.Min(1f + multiplier, MaxShortDamageTotal), 1f);
            }
            orig(self, damageInfo);
            if (self.body == null) return;
            if (damageInfo.HasModdedDamageType(Assets.ShockingDamageType))
            {
                List<CharacterBody.TimedBuff> timedBuffs = self.body.timedBuffs;
                foreach (CharacterBody.TimedBuff timedBuff in timedBuffs) timedBuff.timer = timedBuff.totalDuration;
                self.body.AddTimedBuff(Assets.Shocking, ShockingTimer);
            }
        }
        private static void CharacterBody_HandleCascadingBuffs(On.RoR2.CharacterBody.orig_HandleCascadingBuffs orig, RoR2.CharacterBody self)
        {
            orig(self);
            if (self.GetBuffCount(Assets.Shocking) >= ShockAmmount)
            {
                SetStateOnHurt setStateOnHurt = self.GetComponent<SetStateOnHurt>();
                if (setStateOnHurt != null)
                {
                    setStateOnHurt.SetShock(ShockDuration);
                }
                self.SetBuffCount(Assets.Shocking.buffIndex, 0);
            }
        }
    }
}
