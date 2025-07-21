using EntityStates;
using EntityStates.Toolbot;
using HG;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SicarianInfiltrator
{
    public class FireFlechet : BaseSkillState
    {
        public static float damageCoefficient => FireFlechetConfig.damageCoefficient.Value;
        public static float procCoefficient => FireFlechetConfig.procCoefficient.Value;
        public static float maxDistance => FireFlechetConfig.maxDistance.Value;
        public static float spreadBloomValue => FireFlechetConfig.spreadBloomValue.Value;
        public static float force => FireFlechetConfig.force.Value;
        public static float baseDuration => FireFlechetConfig.baseDuration.Value;
        public static float fireRateIncreaseOverTime => FireFlechetConfig.fireRateIncreaseOverTime.Value;
        public static float horizontalSpreadIncreaseOverTime => FireFlechetConfig.horizontalSpreadIncreaseOverTime.Value;
        public static float verticalSpreadIncreaseOverTime => FireFlechetConfig.verticalSpreadIncreaseOverTime.Value;
        public static float maxIncreasedFireRate => FireFlechetConfig.maxIncreasedFireRate.Value;
        public static float maxIncreasedHorizontalSpread => FireFlechetConfig.maxIncreasedHorizontalSpread.Value;
        public static float maxIncreasedVerticalSpread => FireFlechetConfig.maxIncreasedVerticalSpread.Value;
        public static float minSpread => FireFlechetConfig.minSpread.Value;
        public static int bulletCount => FireFlechetConfig.bulletCount.Value;
        public static float animationDuration = 1.5f;
        public static float animationTransition = 0.1f;
        public float duration;
        public float stopwatch;
        public void FireBullet(Ray aimRay, int bulletCount, float spreadPitchScale, float spreadYawScale)
        {
            base.StartAimMode(aimRay, 3f, false);
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack
                {
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    bulletCount = (uint)bulletCount,
                    damage = this.damageStat * damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageTypeCombo.GenericPrimary,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    force = force,
                    HitEffectNormal = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    maxDistance = maxDistance,
                    radius = 0.1f,
                    isCrit = base.RollCrit(),
                    muzzleName = "Muzzle",
                    minSpread = minSpread,
                    hitEffectPrefab = BaseNailgunState.hitEffectPrefab,
                    maxSpread = base.characterBody.spreadBloomAngle,
                    smartCollision = false,
                    sniper = false,
                    spreadPitchScale = spreadPitchScale,
                    spreadYawScale = spreadYawScale,
                    tracerEffectPrefab = BaseNailgunState.tracerEffectPrefab,
                    allowTrajectoryAimAssist = false
                };
                bulletAttack.AddModdedDamageType(Assets.ShockingDamageType);
                bulletAttack.Fire();
                activatorSkillSlot.stock--;
                characterBody.OnSkillActivated(activatorSkillSlot);
            }
            if (base.characterBody)
            {
                base.characterBody.AddSpreadBloom(spreadBloomValue);
            }
            Util.PlaySound(BaseNailgunState.fireSoundString, base.gameObject);
            PlayAnimation("LeftHand, Override", "Shoot", "Shoot.playbackRate", animationDuration, animationTransition);
            PlayAnimation("LeftHand, Additive", "Shoot");
            EffectManager.SimpleMuzzleFlash(BaseNailgunState.muzzleFlashPrefab, base.gameObject, BaseNailgunState.muzzleName, false);
        }
        public float CalculateDuration()
        {
            return baseDuration / characterBody.attackSpeed / Mathf.Min((fixedAge * fireRateIncreaseOverTime + 1f), maxIncreasedFireRate);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            duration = CalculateDuration();
            stopwatch = duration;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += base.GetDeltaTime();
            if (this.stopwatch >= duration)
            {
                if (!base.isAuthority || (base.IsKeyDownAuthority() && activatorSkillSlot.stock > 0))
                {
                }
                else
                {
                    this.outer.SetStateToMain();
                    return;
                }
                int fireAmount = 0;
                while (this.stopwatch >= duration)
                {
                    stopwatch -= duration;
                    fireAmount++;
                }
                Ray aimRay = base.GetAimRay();
                TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref aimRay, maxDistance, base.gameObject, 1f);
                Vector3 direction = aimRay.direction;
                FireBullet(aimRay, fireAmount * bulletCount, MathF.Min(fixedAge * verticalSpreadIncreaseOverTime * characterBody.attackSpeed, maxIncreasedVerticalSpread), MathF.Min(fixedAge * horizontalSpreadIncreaseOverTime * characterBody.attackSpeed, maxIncreasedHorizontalSpread));
                duration = CalculateDuration();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public abstract class BaseSwingTaserGoad : BaseSkillState
    {
        public abstract float baseDuration { get; }
        public abstract float damageCoefficient {get;}
        public abstract float procCoefficient { get; }
        public abstract string hitboxName { get; }
        public static float force => TaserGoadConfig.force.Value;
        public static float baseAcceleration => TaserGoadConfig.baseAcceleration.Value;
        public static float baseCurrent => TaserGoadConfig.baseCurrent.Value;
        public static float baseMaxSpeed => TaserGoadConfig.baseMaxSpeed.Value;
        public static float baseInvincivilityWindow => TaserGoadConfig.baseInvincibilityWindow.Value;
        public static bool addLightArmor => TaserGoadConfig.addLightArmor.Value;
        public Vector3 direction;
        public float duration;
        public HitBoxGroup hitBoxGroup;
        public OverlapAttack overlapAttack;
        public float acceleration;
        public float current;
        public bool armorAdded;
        public Vector3 currentVector;
        public bool crit;
        public abstract void SetNextState();
        public override void OnEnter()
        {
            base.OnEnter();
            SetupOverlapAttack();
            duration = baseDuration / attackSpeedStat;
            acceleration = moveSpeedStat * baseAcceleration;
            current = baseCurrent;
            Ray ray = base.GetAimRay();
            direction = new Vector3(ray.direction.x, 0f, ray.direction.z);
            StartAimMode(2f, true);
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(DLC2Content.Buffs.HiddenRejectAllDamage, baseInvincivilityWindow);
                if (addLightArmor)
                {
                    characterBody.AddBuff(RoR2Content.Buffs.SmallArmorBoost);
                    armorAdded = true;
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                if(armorAdded)
                characterBody.RemoveBuff(RoR2Content.Buffs.SmallArmorBoost);
            }
        }
        public void SetupOverlapAttack()
        {
            OverlapAttack overlapAttack = new OverlapAttack();
            overlapAttack.attacker = base.gameObject;
            overlapAttack.damage = characterBody.damage;
            overlapAttack.damageColorIndex = DamageColorIndex.Default;
            overlapAttack.damageType = new DamageTypeCombo(DamageType.Shock5s, DamageTypeExtended.Generic, DamageSource.Secondary);
            overlapAttack.forceVector = transform.forward;
            overlapAttack.hitBoxGroup = null;
            //overlapAttack.hitEffectPrefab = this.hitEffectPrefab;
            //NetworkSoundEventDef networkSoundEventDef = this.impactSound;
            //overlapAttack.impactSound = ((networkSoundEventDef != null) ? networkSoundEventDef.index : NetworkSoundEventIndex.Invalid);
            overlapAttack.inflictor = base.gameObject;
            overlapAttack.isCrit = false;
            overlapAttack.procChainMask = default(ProcChainMask);
            overlapAttack.pushAwayForce = force;
            overlapAttack.procCoefficient = 1f;
            overlapAttack.teamIndex = base.GetTeam();
            this.overlapAttack = overlapAttack;
        }
        public void Swing(Vector3 vector3, float damageMultiplier, bool crit, float procCoefficient, string hitboxName)
        {
            if (base.isAuthority)
            {
                this.hitBoxGroup = base.FindHitBoxGroup(hitboxName);
                if (this.hitBoxGroup)
                {
                    overlapAttack.forceVector= vector3;
                    overlapAttack.hitBoxGroup = this.hitBoxGroup;
                    overlapAttack.damage = characterBody.damage * damageMultiplier;
                    overlapAttack.isCrit = crit;
                    overlapAttack.procCoefficient = procCoefficient;
                    overlapAttack.Fire();
                }
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode();
            current = Mathf.SmoothDamp(current, 0f, ref acceleration, baseMaxSpeed);
            if (isAuthority)
            {
                if (characterMotor)
                {
                    characterMotor.velocity = direction * characterBody.moveSpeed * current;
                }
                else if (rigidbody)
                {
                    rigidbody.velocity = direction * characterBody.moveSpeed * current;
                }
                if (fixedAge < baseDuration) Swing(direction, damageCoefficient, crit, procCoefficient, hitboxName);
                if (fixedAge >= duration && !IsKeyDownAuthority()) SetNextState();
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
    public class SwingTaserGoad : BaseSwingTaserGoad
    {
        public override float damageCoefficient => TaserGoadConfig.firstSwingDamageCoefficient.Value;
        public override float procCoefficient => TaserGoadConfig.firstSwingProcCoefficient.Value;
        public override float baseDuration => TaserGoadConfig.firstSwingBaseDuration.Value;
        public override string hitboxName => "TaserClose";
        public static Vector3 effectScale = new Vector3(1f, 1f, 1f);
        public static string enterSound = "Play_loader_m1_swing";
        public bool end = true;
        public override void OnEnter()
        {
            base.OnEnter();
            crit = RollCrit();
            PlayAnimation("UpperBody, Override", "Slash1", "Slash.playbackRate", 0.583f);
            Util.PlaySound(enterSound, base.gameObject);
            GameObject effect = GameObject.Instantiate(Assets.TaserSlash);
            effect.transform.SetParent(modelLocator.modelTransform, false);
            effect.transform.localScale = effectScale;
        }
        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("UpperBody, Override", "BufferEmpty", "Slash.playbackRate", 1f, 1f);
        }
        public override void SetNextState()
        {
            outer.SetNextState(new SwingTaserGoadSecond { activatorSkillSlot = activatorSkillSlot, crit = crit});
        }
    }
    public class SwingTaserGoadSecond : BaseSwingTaserGoad
    {
        public override float damageCoefficient => TaserGoadConfig.secondSwingDamageCoefficient.Value;
        public override float procCoefficient => TaserGoadConfig.secondSwingProcCoefficient.Value;
        public override float baseDuration => TaserGoadConfig.secondSwingBaseDuration.Value;
        public override string hitboxName => "TaserFar";
        public static Vector3 effectScale = new Vector3(-1f, 1f, 1.5f);
        public static string enterSound = "Play_loader_m1_swing";
        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("UpperBody, Override", "Slash2", "Slash.playbackRate", 0.583f);
            Util.PlaySound(enterSound, base.gameObject);
            GameObject effect = GameObject.Instantiate(Assets.TaserSlash);
            effect.transform.SetParent(modelLocator.modelTransform, false);
            effect.transform.localScale = effectScale;
        }
        public override void OnExit()
        {
            base.OnExit();
            //PlayAnimation("UpperBody, Override", "BufferEmpty");
        }
        public override void SetNextState()
        {
            outer.SetNextStateToMain();
        }
    }
    public class DomeStomp : GenericCharacterMain
    {
        public static float damageCoefficient => HelmetSlamConfig.damageCoefficient.Value;
        public static float procCoefficient => HelmetSlamConfig.procCoefficient.Value;
        public static float force => HelmetSlamConfig.force.Value;
        public static float bonusForce => HelmetSlamConfig.bonusForce.Value;
        public static float radius => HelmetSlamConfig.radius.Value;
        public static float jumpPower => HelmetSlamConfig.jumpPower.Value;
        public static float gravityScale => HelmetSlamConfig.gravityScale.Value;
        public static float walkSpeedPenalty => HelmetSlamConfig.walkSpeedPenalty.Value;
        public static float indicatorSmoothTime = 0.2f;
        public static float timeUntilFixedUpdateCheck = 0.1f;
        public static BlastAttack.FalloffModel slamDamageFalloff => HelmetSlamConfig.slamDamageFalloff.Value;
        public static string enterSound = "Play_loader_R_variant_activate";
        public static string exitSound = "Play_loader_R_variant_slam";
        public float gravityScaleDelta;
        public float walkSpeedPenaltyDelta;
        public bool fire = true;
        public GameObject indicator;
        public float indicatorAcceleration;
        public Vector3 gravity
        {
            get { return characterMotor ? new Vector3(0f, Physics.gravity.y, 0f) : Physics.gravity ;}
        }
        public override void ProcessJump()
        {
        }
        public void PlaceIndicator()
        {
            if (indicator == null) return;
            if (Physics.Raycast(characterBody.footPosition, gravity.normalized, out RaycastHit raycastHit, 99999f, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
            {
                if (!indicator.activeSelf)
                    indicator.SetActive(true);
                indicator.transform.position = raycastHit.point;
            }
            else
            {
                if (indicator.activeSelf)
                    indicator.SetActive(false);
            }
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(enterSound, base.gameObject);
            //PlayAnimation("FullBody, Override", "Flip");
            int layerIndex = modelAnimator.GetLayerIndex("Body");
            if(layerIndex >= 0)
            modelAnimator.CrossFadeInFixedTime("Jump", smoothingParameters.intoJumpTransitionTime, layerIndex);
            PlayAnimation("FullBody, Additive", "FlipEnter", "Slam.playbackRate", 0.5f);
            indicator = GameObject.Instantiate(Assets.Indicator);
            indicator.transform.localScale = Vector3.zero;
            PlaceIndicator();
            if (characterMotor)
            {
                if (isAuthority)
                {
                    characterMotor.velocity.y = gravity.y * -1f * jumpPower;
                    characterMotor.onHitGroundAuthority += CharacterMotor_onHitGroundAuthority;
                }
                if(NetworkServer.active)
                characterBody.AddBuff(JunkContent.Buffs.IgnoreFallDamage);
                gravityScaleDelta = characterMotor.gravityScale;
                characterMotor.gravityScale = gravityScale;
                characterMotor.airControl *= attackSpeedStat;
                walkSpeedPenaltyDelta = characterMotor.walkSpeedPenaltyCoefficient;
                characterMotor.walkSpeedPenaltyCoefficient = walkSpeedPenalty;
            }
            else if(isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= timeUntilFixedUpdateCheck && isAuthority && characterMotor && characterMotor.isGrounded) FireExplosion();
        }
        
        public override void Update()
        {
            base.Update();
            float scale = Mathf.SmoothDamp(transform.localScale.x, radius, ref indicatorAcceleration, indicatorSmoothTime * Time.deltaTime);
            indicator.transform.localScale = new Vector3(scale, scale, scale);
            PlaceIndicator();
        }
        public override void OnExit()
        {
            base.OnExit();
            //PlayAnimation("FullBody, Additive", "FlipExit");
            PlayAnimation("FullBody, Additive", "BufferEmpty", "Slam.playbackRate", 0.1f, 0f);
            if (NetworkServer.active)
                characterBody.RemoveBuff(JunkContent.Buffs.IgnoreFallDamage);
            if (characterMotor)
            {
                if(isAuthority) characterMotor.onHitGroundAuthority -= CharacterMotor_onHitGroundAuthority;
                characterMotor.airControl /= attackSpeedStat;
                characterMotor.gravityScale = gravityScaleDelta;
                characterMotor.walkSpeedPenaltyCoefficient = walkSpeedPenaltyDelta;
            }
            if(indicator) Destroy(indicator);
        }
        public void FireExplosion()
        {
            if (!fire || !isAuthority) return;
            BlastAttack blasAttack = new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = teamComponent ? teamComponent.teamIndex : TeamIndex.None,
                baseDamage = characterBody.damage * damageCoefficient,
                baseForce = force * 0.2f,
                position = characterBody.footPosition,
                radius = radius,
                falloffModel = BlastAttack.FalloffModel.Linear,
                bonusForce = gravity * -1f * bonusForce,
                procCoefficient = procCoefficient,
                damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, DamageSource.Utility)
            };
            blasAttack.Fire();
            fire = false;
            EffectData effectData = new EffectData
            {
                origin = blasAttack.position,
                scale = blasAttack.radius
            };
            EffectManager.SpawnEffect(Assets.Slam, effectData, true);
            EffectData effectData2 = new EffectData
            {
                origin = blasAttack.position,
                scale = blasAttack.radius
            };
            EffectManager.SpawnEffect(Assets.LoaderSlam, effectData2, true);
            outer.SetNextStateToMain();
        }
        private void CharacterMotor_onHitGroundAuthority(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            FireExplosion();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }

    public class ThrowARCGrenade : BaseSkillState
    {
        public static float damageCoefficient => ThrowARCGrenadeConfig.damageCoefficient.Value;
        public static float baseDuration => ThrowARCGrenadeConfig.baseDuration.Value;
        public static float baseDistance => ThrowARCGrenadeConfig.baseDistance.Value;
        public static float indicatorSmoothTime = 0.2f;
        public static GameObject projectile = Assets.ARCGrenadeProjectile;
        public static string throwSound = "Play_commando_M2_grenade_throw";
        public bool fire = true;
        public GameObject indicator;
        public float stopwatch;
        public float duration;
        public float distance;
        public float radius;
        public float indicatorAcceleration;
        public float startingVelocity;
        public Vector3 finalIndicatorPosition;
        public ProjectileImpactExplosion projectileImpactExplosion;
        public ProjectileSimple projectileSimple;
        public Dictionary<Collider, HurtBox> keyValuePairs = new Dictionary<Collider, HurtBox>();
        public ChildLocator childLocator;
        public GameObject gun;
        public GameObject grenade;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            indicator = GameObject.Instantiate(Assets.Indicator);
            indicator.transform.localScale = Vector3.zero;
            projectileImpactExplosion = projectile.GetComponent<ProjectileImpactExplosion>();
            //projectileSimple = projectile.GetComponent<ProjectileSimple>();
            radius = projectileImpactExplosion ? projectileImpactExplosion.blastRadius : 0f;
            //startingVelocity = projectileSimple ? projectileSimple.desiredForwardSpeed : 0f;
            PlayAnimation("LeftHand, Override", "SelectGrenade");
            childLocator = modelLocator && modelLocator.modelTransform ? modelLocator.modelTransform.GetComponent<ChildLocator>() : null;
            if (childLocator)
            {
                gun = childLocator.FindChild("Gun").gameObject;
                grenade = childLocator.FindChild("Grenade").gameObject;
            }
            if (gun) gun.SetActive(false);
            if (grenade) grenade.SetActive(true);
        }
        public void PlaceIndicator(Ray ray)
        {
            if (indicator == null) return;
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, baseDistance, LayerIndex.world.mask + LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
            bool hit = false;
            distance = baseDistance;
            RaycastHit raycastHit1 = new RaycastHit();
            if (raycastHits != null && raycastHits.Length > 0)
                foreach (RaycastHit raycastHit in raycastHits)
                {
                    if (raycastHit.distance < distance)
                    {
                        HurtBox hurtBox;
                        if (keyValuePairs.ContainsKey(raycastHit.collider))
                        {
                            hurtBox = keyValuePairs[raycastHit.collider];
                        }
                        else
                        {
                            hurtBox = raycastHit.collider.GetComponent<HurtBox>();
                            keyValuePairs.Add(raycastHit.collider, hurtBox);
                        }
                        if (hurtBox && teamComponent)
                        {
                            CharacterBody characterBody = hurtBox.healthComponent ? hurtBox.healthComponent.body : null;
                            if (characterBody && characterBody.teamComponent && characterBody.teamComponent.teamIndex != teamComponent.teamIndex)
                            {
                                Catch(ref hit, ref raycastHit1, raycastHit);
                            }
                        }
                        else
                        {
                            Catch(ref hit, ref raycastHit1, raycastHit);
                        }
                    }
                }
            if (hit)
            {
                indicator.transform.position = raycastHit1.point;
            }
            else
            {
                indicator.transform.position = ray.origin + ray.direction * distance;
            }
        }
        private void Catch(ref bool hit, ref RaycastHit raycastHit, RaycastHit raycastHit1)
        {
            raycastHit = raycastHit1;
            distance = raycastHit.distance;
            hit = true;
        }
        public void FireProjectile()
        {
            fire = false;
            Ray ray = GetAimRay();
            Util.PlaySound(throwSound, base.gameObject);
            PlayAnimation("LeftHand, Override", "ThrowGrenade");
            if (grenade) grenade.SetActive(false);
            PlaceIndicator(ray);
            if (base.isAuthority)
            {
                TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectile, gameObject, 1f);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = projectile,
                    position = ray.origin,
                    rotation = Util.QuaternionSafeLookRotation(ray.direction),
                    owner = base.gameObject,
                    damage = damageCoefficient * characterBody.damage,
                    force = 0f,
                    crit = RollCrit(),
                    damageTypeOverride = new DamageTypeCombo?(new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Special))
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }
        public override void Update()
        {
            base.Update();
            if (!fire) return;
            Ray ray = GetAimRay();
            float scale = Mathf.SmoothDamp(transform.localScale.x, radius, ref indicatorAcceleration, indicatorSmoothTime * Time.deltaTime);
            indicator.transform.localScale = new Vector3(scale, scale, scale);
            PlaceIndicator(ray);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fire && ( !base.isAuthority || (base.IsKeyDownAuthority())))
            {
                return;
            }
            if (fire) FireProjectile();
            stopwatch += Time.fixedDeltaTime;
            if(stopwatch >= baseDuration)
            outer.SetNextStateToMain();
        }
        public override void OnExit()
        {
            base.OnExit();
            if (gun) gun.SetActive(true);
            if (grenade) grenade.SetActive(false);
            //if (indicator) Destroy(indicator);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
