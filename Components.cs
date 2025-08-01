using EntityStates;
using EntityStates.Toolbot;
using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SicarianInfiltrator
{
    public class ARCGrenadeComponent : MonoBehaviour
    {
        [HideInInspector] public ThrowARCGrenade throwARCGrenadeState;
        public ProjectileController projectileController;
        public ProjectileImpactExplosion projectileImpactExplosion;
        [HideInInspector] public CharacterBody characterBody;
        [HideInInspector] public GameObject indicator;
        public void Start()
        {
            if (projectileController == null) return;
            characterBody = projectileController.owner ? projectileController.owner.GetComponent<CharacterBody>() : null;
            GenericSkill[] genericSkills = characterBody.skillLocator ? characterBody.skillLocator.allSkills : null;
            if (genericSkills != null && genericSkills.Length > 0)
                foreach (GenericSkill skill in genericSkills)
                {
                    if (skill == null || skill.stateMachine == null || skill.stateMachine.state == null) continue;
                    throwARCGrenadeState = skill.stateMachine.state is ThrowARCGrenade ? skill.stateMachine.state as ThrowARCGrenade : null;
                    if (throwARCGrenadeState != null) break;
                }
            if (projectileImpactExplosion && throwARCGrenadeState != null)
            {
                indicator = throwARCGrenadeState.indicator;
                if(throwARCGrenadeState.characterBody)
                projectileController.rigidbody.velocity *= throwARCGrenadeState.characterBody.attackSpeed;
                projectileImpactExplosion.lifetime = Trajectory.CalculateGroundTravelTime(projectileController.rigidbody.velocity.magnitude, throwARCGrenadeState.distance);
                if (projectileController.rigidbody.useGravity)
                {
                    Vector3 vector3 = projectileController.rigidbody.velocity;
                    vector3.y += Trajectory.CalculateInitialYSpeedForFlightDuration(projectileImpactExplosion.lifetime);
                    projectileController.rigidbody.velocity = vector3;
                }
            }
                
        }
        public void OnEnable()
        {
            if (projectileImpactExplosion)
                projectileImpactExplosion.OnProjectileExplosion += OnProjectileExplosion;
        }
        public void OnDisable()
        {
            if (projectileImpactExplosion)
                projectileImpactExplosion.OnProjectileExplosion -= OnProjectileExplosion;
        }
        public void OnDestroy()
        {
            if (indicator) Destroy(indicator);
        }
        private void OnProjectileExplosion(BlastAttack attack, BlastAttack.Result result)
        {
        }
    }
    public class UntargetableComponent : MonoBehaviour
    {
        public CharacterBody characterBody;
        public CharacterModel characterModel;
        public Inventory inventory;
        public static float shortDamageDuration => UntargetableConfig.shortDamageDuration.Value;
        public void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            characterModel = characterBody && characterBody.modelLocator && characterBody.modelLocator.modelTransform ? characterBody.modelLocator.modelTransform.GetComponent<CharacterModel>() : null;
            inventory = characterBody.inventory;
            serverActive = NetworkServer.active;
        }
        private void SetProvidingBuff(bool shouldProvide)
        {
            if (shouldProvide == providing || characterBody == null)
            {
                return;
            }
            providing = shouldProvide;
            if (providing)
            {
                if (serverActive)
                {
                    characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                    characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                    characterBody.ClearTimedBuffs(Assets.ShortDamage);
                    characterBody.AddBuff(Assets.ShortDamage);
                }
                if (characterModel) AddOverlay(shortDamageDuration);
                return;
            }
            if (characterModel) AddOverlay(shortDamageDuration);
            if (serverActive)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
                characterBody.SetBuffCount(Assets.ShortDamage.buffIndex, 0);
                characterBody.AddTimedBuff(Assets.ShortDamage, shortDamageDuration);
            }
        }
        public void AddOverlay(float duration)
        {
            TemporaryOverlay temporaryOverlay = gameObject.AddComponent<TemporaryOverlay>();
            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            temporaryOverlay.animateShaderAlpha = true;
            temporaryOverlay.destroyComponentOnEnd = true;
            temporaryOverlay.duration = duration;
            temporaryOverlay.originalMaterial = Assets.RoboballMaterial;
            temporaryOverlay.inspectorCharacterModel = characterModel;
            temporaryOverlay.AddToCharacerModel(characterModel);
        }
        private void OnDisable()
        {
            SetProvidingBuff(false);
        }
        private void FixedUpdate()
        {
            SetProvidingBuff(characterBody.outOfCombat);
        }
        public bool serverActive;
        public bool providing;
    }
    [CreateAssetMenu(menuName = "RoR2/SkillDef/UntargetableSkillDef")]
    public class UntargetableSkillDef : SkillDef
    {
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            UntargetableSkillInstanceData untargetableSkillInstanceData = new UntargetableSkillInstanceData
            {
                untargetableComponent = skillSlot.gameObject.GetOrAddComponent<UntargetableComponent>()
            };
            return untargetableSkillInstanceData;
        }
        public override void OnUnassigned([NotNull] GenericSkill skillSlot)
        {
            UntargetableSkillInstanceData untargetableSkillInstanceData = skillSlot.skillInstanceData != null && skillSlot.skillInstanceData is UntargetableSkillInstanceData ? skillSlot.skillInstanceData as UntargetableSkillInstanceData : null;
            UntargetableComponent untargetableComponent = untargetableSkillInstanceData != null ? untargetableSkillInstanceData .untargetableComponent : skillSlot.gameObject.GetComponent<UntargetableComponent>();
            if (untargetableComponent) Destroy(untargetableComponent);
            base.OnUnassigned(skillSlot);
        }
        public class UntargetableSkillInstanceData : BaseSkillInstanceData
        {
            public UntargetableComponent untargetableComponent;
        }
    }
}