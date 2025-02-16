using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Utility.Extension;
using UnityEngine;

public class NormalAttack : ScriptableObject
{
    // [field: Title("On hit enemy effects")]
    // [field: SerializeField]
    // public List<StatusEffectStack> ApplyToHit { get; set; } = new List<StatusEffectStack>();
    //
    // [Title("Stats")]
    // [field: SerializeField]
    // public List<Attack> AttackList { get; set; } = new List<Attack>();
    //
    // private Entity MyTargetOverride { get; set; }
    // private Entity WasHitTarget { get; set; }
    // private EffectSpeed MyEffectSpeed { get; set; }
    // private bool AllowInDeath { get; set; }
    // private Entity MyEntity { get; set; }
    // private Attack CurrentAttack { get; set; }
    //
    // private readonly int m_MaxTriggers = 50;
    //
    // private int m_CurrentTriggers;
    //
    // public bool DontLimitUses { get; set; }
    //
    // private bool ApplyAttackEffects { get; set; }
    //
    // private int CurrentAction
    // {
    //     get => MyEntity.CurrentAction;
    //     set => MyEntity.CurrentAction = value;
    // }
    //
    // public int MaxTriggers => m_MaxTriggers;
    //
    // public int CurrentTriggers => m_CurrentTriggers;
    //
    // public bool IgnoreDeath
    // {
    //     get
    //     {
    //         if (MyEffectSpeed != EffectSpeed.OnMyDeath && MyEffectSpeed != EffectSpeed.OnAnyDeath)
    //         {
    //             return AllowInDeath;
    //         }
    //
    //         return true;
    //     }
    // }
    //
    // public bool Tick(Entity entity, EffectSpeed effectSpeed, bool isPassive = false, Entity targetOverride = null,
    //     bool allowDeath = false, Entity wasHit = null)
    // {
    //     MyEntity = entity;
    //     MyTargetOverride = targetOverride;
    //     MyEffectSpeed = effectSpeed;
    //     AllowInDeath = allowDeath;
    //     WasHitTarget = wasHit;
    //     if (entity != null && entity.HasEffect(StatusEffect.Asleep) && !isPassive)
    //     {
    //         entity.ConsumeEffect(StatusEffect.Asleep, 1);
    //         return true;
    //     }
    //
    //     int num = 1;
    //
    //     if (m_CurrentTriggers >= m_MaxTriggers && !DontLimitUses)
    //     {
    //         return true;
    //     }
    //
    //     m_CurrentTriggers++;
    //
    //     bool result = false;
    //     for (int i = 0; i < num; i++)
    //     {
    //         ApplyAttackEffects = true;
    //         foreach (Attack attack2 in AttackList)
    //         {
    //             Attack attack = CurrentAttack = attack2;
    //             switch (attack.AttackEffect)
    //             {
    //                 case AttackEffect.PlayerHealth:
    //                     DoAttack(entity, attack, attack.TargetType, PlayerHealth, Target);
    //                     break;
    //                 case AttackEffect.None:
    //                     DoAttack(entity, attack, attack.TargetType, ApplyEffects, Target);
    //                     break;
    //                 case AttackEffect.TriggerDecay:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerDecay, Target);
    //                     break;
    //                 case AttackEffect.MoveSelf:
    //                     MyTargetOverride = null;
    //                     DoAttack(entity, attack, attack.TargetType, MoveSelf, Target);
    //                     break;
    //                 case AttackEffect.PushBack:
    //                     DoAttack(entity, attack, attack.TargetType, PushBack, Target);
    //                     break;
    //                 case AttackEffect.MoveToRandom:
    //                     DoAttack(entity, attack, attack.TargetType, PushBack, Target);
    //                     break;
    //                 case AttackEffect.Energy:
    //                     DoAttack(entity, attack, attack.TargetType, GiveEnergy, Target);
    //                     break;
    //                 case AttackEffect.Damage:
    //                     DoAttack(entity, attack, attack.TargetType, DealDamage, Target);
    //                     break;
    //                 case AttackEffect.Heal:
    //                     DoAttack(entity, attack, attack.TargetType, HealUnits, Target);
    //                     break;
    //                 case AttackEffect.BonusHealth:
    //                     DoAttack(entity, attack, attack.TargetType, BonusHealthUnits, Target);
    //                     break;
    //                 case AttackEffect.AllSummonsDealDamage:
    //                     DoAttack(entity, attack, attack.TargetType, AllSummonsDealDamage, Target);
    //                     break;
    //                 case AttackEffect.Shield:
    //                     DoAttack(entity, attack, attack.TargetType, ShieldUnits, Target);
    //                     break;
    //                 case AttackEffect.Exp:
    //                     DoAttack(entity, attack, attack.TargetType, GrantEXP, Target);
    //                     break;
    //                 case AttackEffect.GainGold:
    //                     DoAttack(entity, attack, attack.TargetType, GrantGold, Target);
    //                     break;
    //                 case AttackEffect.Summon:
    //                     DoAttack(entity, attack, attack.TargetType, SummonUnit, Target);
    //                     break;
    //                 case AttackEffect.SpawnAllBox:
    //                     DoAttack(entity, attack, attack.TargetType, SummonAllBoxUnit, Target);
    //                     break;
    //                 case AttackEffect.SpawnAllSold:
    //                     DoAttack(entity, attack, attack.TargetType, SpawnAllReleased, Target);
    //                     break;
    //                 case AttackEffect.SummonRandomBoxUnit:
    //                     DoAttack(entity, attack, attack.TargetType, SummonRandomBoxUnit, Target);
    //                     break;
    //                 case AttackEffect.SummonSelfLowerLevel:
    //                     DoAttack(entity, attack, attack.TargetType, SummonSelfLowerLevel, Target);
    //                     break;
    //                 case AttackEffect.SummonEnemy:
    //                     DoAttack(entity, attack, attack.TargetType, SummonEnemyUnit, Target);
    //                     break;
    //                 case AttackEffect.TriggerAttack:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerAttack, Target);
    //                     break;
    //                 case AttackEffect.TriggerSuper:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerSuper, Target);
    //                     break;
    //                 case AttackEffect.GainStartEffect:
    //                     DoAttack(entity, attack, attack.TargetType, GainStartEffect, Target);
    //                     break;
    //                 case AttackEffect.RemoveShieldPercent:
    //                     DoAttack(entity, attack, attack.TargetType, ReduceShield, Target);
    //                     break;
    //                 case AttackEffect.CopyEffectInFront:
    //                     DoAttack(entity, attack, attack.TargetType, CopyAndTriggerEffect, Target);
    //                     break;
    //                 case AttackEffect.RandomEnemyEffect:
    //                     DoAttack(entity, attack, attack.TargetType, EnemyEffect, Target);
    //                     break;
    //                 case AttackEffect.EatAndStore:
    //                     DoAttack(entity, attack, attack.TargetType, EatUnit, Target);
    //                     break;
    //                 case AttackEffect.TriggerOnAttack:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerOnAttack, Target);
    //                     break;
    //                 case AttackEffect.TriggerOnHit:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerOnHit, Target);
    //                     break;
    //                 case AttackEffect.TriggerDizzy:
    //                     DoAttack(entity, attack, attack.TargetType, TriggerOnHit, Target);
    //                     break;
    //             }
    //         }
    //
    //         result = true;
    //     }
    //
    //     return result;
    // }
    //
    // public void DoAttack(Entity entity, Attack attack, Targeting targets, Action<Attack, Entity, Entity> onComplete,
    //     Action<Entity, Attack, Targeting, Action<Attack, Entity, Entity>> run)
    // {
    //     List<Entity> list = MyTargetOverride == null
    //         ? GetTargetsAvailable(targets, entity)
    //         : new List<Entity> { MyTargetOverride };
    //     if (list != null && list.Count != 0)
    //     {
    //         run(entity, attack, targets, onComplete);
    //     }
    // }
    //
    // public List<Entity> GetTargetsAvailable(Targeting targetType, Entity self)
    // {
    //     List<Entity> targets = new List<Entity>();
    //     if (self.HasEffect(StatusEffect.Fear))
    //     {
    //         List<Entity> allAlly = BattleManager.GetAllAllyIgnoreSelf(self);
    //         targets.Add(allAlly.Count > 0 ? allAlly.GetRandomElement() : self);
    //         return targets;
    //     }
    //
    //     switch (targetType)
    //     {
    //         case Targeting.AllButSelf:
    //             foreach (Entity item in BattleManager.GetAllAllyIgnoreSelf(self))
    //             {
    //                 AddTarget(item);
    //             }
    //             foreach (Entity item2 in BattleManager.GetAllEnemy(self))
    //             {
    //                 AddTarget(item2);
    //             }
    //
    //             break;
    //         case Targeting.AnyInFrontPerValue1:
    //         {
    //             int num = 0;
    //             foreach (Entity item3 in BattleManager.GetAllAllyInFrontReverse(self))
    //             {
    //                 if (item3 != null && num <= CurrentAttack.value && item3.IsAlive)
    //                 {
    //                     AddTarget(item3);
    //                     num++;
    //                 }
    //             }
    //
    //             foreach (Entity item4 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item4 != null && num <= CurrentAttack.value && item4.IsAlive)
    //                 {
    //                     AddTarget(item4);
    //                     num++;
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.WAS_HIT:
    //             AddTarget(WasHitTarget);
    //             break;
    //         case Targeting.FULL_RANDOM_PER_VALUE3:
    //         {
    //             for (int j = 0; j < CurrentAttack.Value3; j++)
    //             {
    //                 AddTarget(BattleManager.GetFullRandom(self));
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RANDOM_ALLY_PER_LEVEL:
    //         {
    //             for (int i = 0; i < MyEntity.unitScriptable.Level + 1; i++)
    //             {
    //                 AddTarget(BattleManager.GetRandomAlly(MyEntity));
    //             }
    //
    //             break;
    //         }
    //         case Targeting.SELF_PER_LEVEL:
    //         {
    //             for (int l = 0; l < MyEntity.unitScriptable.Level + 1; l++)
    //             {
    //                 AddTarget(MyEntity);
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RANDOM_ENEMY_PER_LEVEL:
    //         {
    //             for (int k = 0; k < MyEntity.unitScriptable.Level + 1; k++)
    //             {
    //                 AddTarget(BattleManager.GetRandomEnemy(MyEntity));
    //             }
    //
    //             break;
    //         }
    //         case Targeting.IN_FRONT_PER_LEVEL:
    //         {
    //             int num3 = 0;
    //             foreach (Entity item5 in BattleManager.GetAllAllyInFrontReverse(self))
    //             {
    //                 if (item5 != null && num3 <= self.unitScriptable.Level && item5.IsAlive)
    //                 {
    //                     AddTarget(item5);
    //                     num3++;
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.BEHIND_PER_LEVEL:
    //         {
    //             int num2 = 0;
    //             foreach (Entity item6 in BattleManager.GetAllAllyBehind(self))
    //             {
    //                 if (item6 != null && num2 <= self.unitScriptable.Level && item6.IsAlive)
    //                 {
    //                     AddTarget(item6);
    //                     num2++;
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.HIGHEST_HP_ALLY:
    //             AddTarget(BattleManager.HighestHPAlly(self));
    //             break;
    //         case Targeting.HIGHEST_HP_ENEMY:
    //             AddTarget(BattleManager.HighestHPEnemy(self));
    //             break;
    //         case Targeting.LOWEST_HP_ALLY:
    //             AddTarget(BattleManager.LowestHPAlly(self));
    //             break;
    //         case Targeting.LOWEST_MAX_HEALTH_ENEMY:
    //             AddTarget(BattleManager.LowestMaxHPEnemy(self));
    //             break;
    //         case Targeting.LOWEST_HP_ENEMY:
    //             AddTarget(BattleManager.LowestHPEnemy(self));
    //             break;
    //         case Targeting.RANDOM_ENEMY:
    //             AddTarget(BattleManager.GetRandomEnemy(self));
    //             break;
    //         case Targeting.FULL_RANDOM:
    //             AddTarget(BattleManager.GetFullRandom(self));
    //             break;
    //         case Targeting.ALL_ENEMY:
    //             foreach (Entity item7 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item7 != null && item7.IsAlive)
    //                 {
    //                     AddTarget(item7);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.SELF:
    //             AddTarget(self);
    //             break;
    //         case Targeting.RANDOM_ALLY:
    //             AddTarget(BattleManager.GetRandomAlly(self));
    //             break;
    //         case Targeting.ALL_ALLY:
    //             foreach (Entity item8 in BattleManager.GetAllAlly(self))
    //             {
    //                 if (item8 != null && item8.IsAlive)
    //                 {
    //                     AddTarget(item8);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.ALL_ALLY_BUT_SELF:
    //             foreach (Entity item9 in BattleManager.GetAllAlly(self))
    //             {
    //                 if (item9 != null && item9.IsAlive &&
    //                     item9.unitScriptable.battleSlot != self.unitScriptable.battleSlot)
    //                 {
    //                     AddTarget(item9);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.EVERYONE:
    //             foreach (Entity item10 in BattleManager.GetAllAlly(self))
    //             {
    //                 if (item10 != null && item10.IsAlive)
    //                 {
    //                     AddTarget(item10);
    //                 }
    //             }
    //
    //             foreach (Entity item11 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item11 != null && item11.IsAlive)
    //                 {
    //                     AddTarget(item11);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.INFRONT:
    //             foreach (Entity item12 in BattleManager.GetAllAllyInfront(self))
    //             {
    //                 if (item12 != null && item12.IsAlive)
    //                 {
    //                     AddTarget(item12);
    //                 }
    //             }
    //
    //             foreach (Entity item13 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item13 != null && item13.IsAlive)
    //                 {
    //                     AddTarget(item13);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.RIGHT_INFRONT:
    //         {
    //             List<Entity> allAllyInfront = BattleManager.GetAllAllyInfront(self);
    //             allAllyInfront.Reverse();
    //             foreach (Entity item14 in allAllyInfront)
    //             {
    //                 if (item14 != null && item14.IsAlive)
    //                 {
    //                     AddTarget(item14);
    //                     return targets;
    //                 }
    //             }
    //
    //             foreach (Entity item15 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item15 != null && item15.IsAlive)
    //                 {
    //                     AddTarget(item15);
    //                     return targets;
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.ALL_ALLY_INFRONT:
    //             foreach (Entity item16 in BattleManager.GetAllAllyInfront(self))
    //             {
    //                 if (item16 != null && item16.IsAlive)
    //                 {
    //                     AddTarget(item16);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.ALL_BEHIND:
    //             foreach (Entity item17 in BattleManager.GetAllAlly(self))
    //             {
    //                 if (item17 != null && item17.IsAlive && item17.unitScriptable.battleSlot < 4 &&
    //                     item17.unitScriptable.battleSlot > self.unitScriptable.battleSlot)
    //                 {
    //                     AddTarget(item17);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.RIGHT_BEHIND:
    //             AddTarget(BattleManager.GetBehind(self));
    //             break;
    //         case Targeting.ALLY_FRONT_AND_BACK:
    //             foreach (Entity item18 in BattleManager.GetInFrontAndBack(self))
    //             {
    //                 if (item18 != null && item18.IsAlive)
    //                 {
    //                     AddTarget(item18);
    //                 }
    //             }
    //
    //             break;
    //         case Targeting.ALLY_INFRONT:
    //             AddTarget(BattleManager.GetInFront(self));
    //             break;
    //         case Targeting.FRONT_ALLY:
    //             AddTarget(BattleManager.GetFrontAlly(self));
    //             break;
    //         case Targeting.BACK_ALLY:
    //             AddTarget(BattleManager.GetBackAlly(self));
    //             break;
    //         case Targeting.FRONT_ENEMY:
    //             AddTarget(BattleManager.GetFrontEnemy(self));
    //             break;
    //         case Targeting.BACK_ENEMY:
    //             AddTarget(BattleManager.GetBackEnemy(self));
    //             break;
    //         case Targeting.FASTED_ENEMY:
    //             AddTarget(BattleManager.FastestSpeedEnemy(self));
    //             break;
    //         case Targeting.ALL_SUMMONS:
    //             foreach (Entity item19 in BattleManager.AllSummons())
    //             {
    //                 AddTarget(item19);
    //             }
    //
    //             break;
    //         case Targeting.FEARED_ENEMIES:
    //             foreach (Entity item20 in BattleManager.GetAllEnemy(self))
    //             {
    //                 if (item20 != null && item20.IsAlive && item20.HasEffect(STATUS_EFFECT.FEAR))
    //                 {
    //                     AddTarget(item20);
    //                 }
    //             }
    //
    //             break;
    //     }
    //
    //     return targets;
    // }
    //
    // private void Target(Entity entity, Attack attack, Targeting targets, Action<Attack, Entity, Entity> onComplete)
    // {
    //     if (MyTargetOverride != null)
    //     {
    //         onComplete(attack, entity, MyTargetOverride);
    //         return;
    //     }
    //
    //     if (entity.HasEffect(STATUS_EFFECT.FEAR))
    //     {
    //         Entity entity2 = null;
    //         List<Entity> allAlly = BattleManager.GetAllAlly(entity);
    //         if (allAlly.Contains(entity))
    //         {
    //             allAlly.Remove(entity);
    //         }
    //
    //         entity2 = ((allAlly.Count <= 0) ? entity : allAlly[UnityEngine.Random.Range(0, allAlly.Count)]);
    //         onComplete(attack, entity, entity2, skipWaiting);
    //         entity.ConsumeEffect(STATUS_EFFECT.FEAR, 1);
    //         PassiveTriggerHelper.OnFear(entity);
    //         return;
    //     }
    //
    //     switch (targets)
    //     {
    //         case Targeting.ALL_BUT_SELF:
    //             Target(entity, attack, Targeting.ALL_ALLY_BUT_SELF, onComplete, skipWaiting);
    //             Target(entity, attack, Targeting.ALL_ENEMY, onComplete, skipWaiting);
    //             break;
    //         case Targeting.SELF_PER_LEVEL:
    //         {
    //             for (int i = 0; i < MyEntity.unitScriptable.Level + 1; i++)
    //             {
    //                 onComplete(attack, entity, entity, skipWaiting);
    //             }
    //
    //             break;
    //         }
    //         case Targeting.WAS_HIT:
    //             onComplete(attack, entity, WasHitTarget, skipWaiting);
    //             break;
    //         case Targeting.FULL_RANDOM_PER_VALUE3:
    //         {
    //             for (int j = 0; j < CurrentAttack.Value3; j++)
    //             {
    //                 onComplete(attack, entity, BattleManager.GetFullRandom(MyEntity), skipWaiting);
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RANDOM_ALLY_PER_LEVEL:
    //         {
    //             for (int l = 0; l < MyEntity.unitScriptable.Level + 1; l++)
    //             {
    //                 onComplete(attack, entity, BattleManager.GetRandomAlly(MyEntity), skipWaiting);
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RANDOM_ENEMY_PER_LEVEL:
    //         {
    //             for (int k = 0; k < MyEntity.unitScriptable.Level + 1; k++)
    //             {
    //                 onComplete(attack, entity, BattleManager.GetRandomEnemy(MyEntity), skipWaiting);
    //             }
    //
    //             break;
    //         }
    //         case Targeting.IN_FRONT_PER_LEVEL:
    //         {
    //             int num3 = 0;
    //             {
    //                 foreach (Entity item in BattleManager.GetAllAllyInFrontReverse(entity))
    //                 {
    //                     if (item != null && num3 <= entity.unitScriptable.Level && item.IsAlive)
    //                     {
    //                         onComplete(attack, entity, item, skipWaiting);
    //                         num3++;
    //                     }
    //                 }
    //
    //                 break;
    //             }
    //         }
    //         case Targeting.ANY_INFRONT_PER_VALUE1:
    //         {
    //             int num2 = 0;
    //             foreach (Entity item2 in BattleManager.GetAllAllyInFrontReverse(entity))
    //             {
    //                 if (item2 != null && num2 <= CurrentAttack.value && item2.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item2, skipWaiting);
    //                     num2++;
    //                 }
    //             }
    //
    //             {
    //                 foreach (Entity item3 in BattleManager.GetAllEnemy(entity))
    //                 {
    //                     if (item3 != null && num2 <= CurrentAttack.value && item3.IsAlive)
    //                     {
    //                         onComplete(attack, entity, item3, skipWaiting);
    //                         num2++;
    //                     }
    //                 }
    //
    //                 break;
    //             }
    //         }
    //         case Targeting.BEHIND_PER_LEVEL:
    //         {
    //             int num = 0;
    //             {
    //                 foreach (Entity item4 in BattleManager.GetAllAllyBehind(entity))
    //                 {
    //                     if (item4 != null && num <= entity.unitScriptable.Level && item4.IsAlive)
    //                     {
    //                         onComplete(attack, entity, item4, skipWaiting);
    //                         num++;
    //                     }
    //                 }
    //
    //                 break;
    //             }
    //         }
    //         case Targeting.ALLY_REINFORCEMENT:
    //         case Targeting.ENEMY_REINFORCEMENT:
    //             onComplete(attack, entity, null, skipWaiting);
    //             break;
    //         case Targeting.HIGHEST_HP_ALLY:
    //             onComplete(attack, entity, BattleManager.HighestHPAlly(entity), skipWaiting);
    //             break;
    //         case Targeting.HIGHEST_HP_ENEMY:
    //             onComplete(attack, entity, BattleManager.HighestHPEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.LOWEST_HP_ALLY:
    //             onComplete(attack, entity, BattleManager.LowestHPAlly(entity), skipWaiting);
    //             break;
    //         case Targeting.LOWEST_MAX_HEALTH_ENEMY:
    //             onComplete(attack, entity, BattleManager.LowestMaxHPEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.LOWEST_HP_ENEMY:
    //             onComplete(attack, entity, BattleManager.LowestHPEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.FULL_RANDOM:
    //             onComplete(attack, entity, BattleManager.GetFullRandom(entity), skipWaiting);
    //             break;
    //         case Targeting.RANDOM_ENEMY:
    //             onComplete(attack, entity, BattleManager.GetRandomEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.ALL_ENEMY:
    //         {
    //             foreach (Entity item5 in BattleManager.GetAllEnemy(entity))
    //             {
    //                 if (item5 != null && item5.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item5, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.FEARED_ENEMIES:
    //         {
    //             foreach (Entity item6 in BattleManager.GetAllEnemy(entity))
    //             {
    //                 if (item6 != null && item6.IsAlive && item6.HasEffect(STATUS_EFFECT.FEAR))
    //                 {
    //                     onComplete(attack, entity, item6, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.SELF:
    //             onComplete(attack, entity, entity, skipWaiting);
    //             break;
    //         case Targeting.RANDOM_ALLY:
    //             onComplete(attack, entity, BattleManager.GetRandomAlly(entity), skipWaiting);
    //             break;
    //         case Targeting.ALL_ALLY:
    //         {
    //             foreach (Entity item7 in BattleManager.GetAllAlly(entity))
    //             {
    //                 if (item7 != null && item7.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item7, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.ALL_ALLY_BUT_SELF:
    //         {
    //             foreach (Entity item8 in BattleManager.GetAllAlly(entity))
    //             {
    //                 if (item8 != null && item8.IsAlive &&
    //                     item8.unitScriptable.battleSlot != entity.unitScriptable.battleSlot)
    //                 {
    //                     onComplete(attack, entity, item8, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.EVERYONE:
    //             Target(entity, attack, Targeting.ALL_ALLY, onComplete, skipWaiting);
    //             Target(entity, attack, Targeting.ALL_ENEMY, onComplete, skipWaiting);
    //             break;
    //         case Targeting.INFRONT:
    //             foreach (Entity item9 in BattleManager.GetAllAllyInfront(entity))
    //             {
    //                 if (item9 != null && item9.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item9, skipWaiting);
    //                 }
    //             }
    //
    //         {
    //             foreach (Entity item10 in BattleManager.GetAllEnemy(entity))
    //             {
    //                 if (item10 != null && item10.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item10, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RIGHT_INFRONT:
    //         {
    //             List<Entity> allAllyInfront = BattleManager.GetAllAllyInfront(entity);
    //             allAllyInfront.Reverse();
    //             foreach (Entity item11 in allAllyInfront)
    //             {
    //                 if (item11 != null && item11.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item11, skipWaiting);
    //                     return;
    //                 }
    //             }
    //
    //             {
    //                 foreach (Entity item12 in BattleManager.GetAllEnemy(entity))
    //                 {
    //                     if (item12 != null && item12.IsAlive)
    //                     {
    //                         onComplete(attack, entity, item12, skipWaiting);
    //                         break;
    //                     }
    //                 }
    //
    //                 break;
    //             }
    //         }
    //         case Targeting.ALL_ALLY_INFRONT:
    //         {
    //             foreach (Entity item13 in BattleManager.GetAllAllyInfront(entity))
    //             {
    //                 if (item13 != null && item13.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item13, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.ALL_BEHIND:
    //         {
    //             foreach (Entity item14 in BattleManager.GetAllAlly(entity))
    //             {
    //                 if (item14 != null && item14.IsAlive && item14.unitScriptable.battleSlot < 4 &&
    //                     item14.unitScriptable.battleSlot > entity.unitScriptable.battleSlot)
    //                 {
    //                     onComplete(attack, entity, item14, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.RIGHT_BEHIND:
    //             onComplete(attack, entity, BattleManager.GetBehind(entity), skipWaiting);
    //             break;
    //         case Targeting.ALLY_FRONT_AND_BACK:
    //         {
    //             foreach (Entity item15 in BattleManager.GetInFrontAndBack(entity))
    //             {
    //                 if (item15 != null && item15.IsAlive && item15 != entity)
    //                 {
    //                     onComplete(attack, entity, item15, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.ALLY_INFRONT:
    //             onComplete(attack, entity, BattleManager.GetInFront(entity), skipWaiting);
    //             break;
    //         case Targeting.FRONT_ALLY:
    //             onComplete(attack, entity, BattleManager.GetFrontAlly(entity), skipWaiting);
    //             break;
    //         case Targeting.BACK_ALLY:
    //             onComplete(attack, entity, BattleManager.GetBackAlly(entity), skipWaiting);
    //             break;
    //         case Targeting.FRONT_ENEMY:
    //             onComplete(attack, entity, BattleManager.GetFrontEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.BACK_ENEMY:
    //             onComplete(attack, entity, BattleManager.GetBackEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.FASTED_ENEMY:
    //             onComplete(attack, entity, BattleManager.FastestSpeedEnemy(entity), skipWaiting);
    //             break;
    //         case Targeting.ALL_SUMMONS:
    //         {
    //             foreach (Entity item16 in BattleManager.AllSummons())
    //             {
    //                 if (item16 != null && item16.IsAlive)
    //                 {
    //                     onComplete(attack, entity, item16, skipWaiting);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case Targeting.EMPTY_SLOT:
    //         case Targeting.RANDOM_OTHER_ALLY_PER_LEVEL:
    //             break;
    //     }
    // }
    //
    // private void AllSummonsDealDamage(Attack attack, Entity entity, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         Debug.LogError("Target Dead");
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     bool flag = false;
    //     bool first = true;
    //     foreach (Entity item in BattleManager.AllSummons())
    //     {
    //         DoAttack(item);
    //         flag = true;
    //     }
    //
    //     if (!flag)
    //     {
    //         BattleManager.FinishAction();
    //     }
    //
    //     void DoAttack(Entity from)
    //     {
    //         attack.elementType = TYPE.BASE_GAME;
    //         attack.Projectile = StatusEffectHelper.GetDamage();
    //         from.AttackTargetVisual(target, TYPE.NONE, attack.anim, attack, delegate { OnHit(from); });
    //     }
    //
    //     void OnHit(Entity from)
    //     {
    //         target?.GetHit(attack.value, DamageType.Normal, TYPE.NONE, null, null, checkPassive: true, canBlock: true,
    //             from);
    //         if (first)
    //         {
    //             BattleManager.FinishAction();
    //             first = false;
    //         }
    //     }
    // }
    //
    // private void DealDamage(Attack attack, Entity entity, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     TYPE attackType = entity.unitScriptable.unitTypes[0];
    //     float damageEffectiveness = 1f;
    //     damageEffectiveness *= DamageHelper.GetTypesBonus(entity.unitScriptable.unitTypes, target);
    //     int attackValue0 = attack.value;
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         if (!IsPassive)
    //         {
    //             attack.Projectile = RewardManager.Instance.attackDictionary.GetOutSprite(attackType);
    //             attack.elementType = attackType;
    //         }
    //         else
    //         {
    //             attack.Projectile = StatusEffectHelper.GetDamage();
    //             attack.elementType = TYPE.BASE_GAME;
    //         }
    //
    //         Entity entity2 = entity;
    //         Entity target2 = target;
    //         TYPE attackType2 = attackType;
    //         ATTACK_ANIM anim = attack.anim;
    //         Attack attack2 = attack;
    //         Action onHit = OnHit;
    //         bool flag = IsPassive;
    //         float damageAdvantageValue = damageEffectiveness;
    //         int delayTimes = CurrentAction;
    //         entity2.AttackTargetVisual(target2, attackType2, anim, attack2, onHit, null, flag, 30, damageAdvantageValue,
    //             delayTimes);
    //         delayTimes = CurrentAction;
    //         CurrentAction = delayTimes + 1;
    //     }
    //
    //     void OnHit()
    //     {
    //         int num = DamageHelper.DamageToTake(entity, target);
    //         DamageType damageEffectiveness2 = ((damageEffectiveness > 1f)
    //             ? DamageType.Strong
    //             : ((!(damageEffectiveness < 1f)) ? DamageType.Normal : DamageType.Weak));
    //         if (dealExactDamage > -1)
    //         {
    //             num = dealExactDamage;
    //             damageEffectiveness = 0f;
    //         }
    //
    //         if (attackValue0 > 0)
    //         {
    //             num = attackValue0;
    //             damageEffectiveness = 0f;
    //         }
    //
    //         if (!IsPassive)
    //         {
    //             if (entity.HasEffect(STATUS_EFFECT.STRENGTH))
    //             {
    //                 int effectStacks = entity.GetEffectStacks(STATUS_EFFECT.STRENGTH);
    //                 num += effectStacks;
    //                 entity.RemoveEffect(STATUS_EFFECT.STRENGTH);
    //                 StaticQuestUnlocker.AddValue(QUEST_VALUES.STRENGTH_USED, effectStacks);
    //             }
    //
    //             if (target.HasEffect(STATUS_EFFECT.FRAIL))
    //             {
    //                 num += target.GetEffectStacks(STATUS_EFFECT.FRAIL);
    //             }
    //
    //             if (target.HasEffect(STATUS_EFFECT.FATIGUE))
    //             {
    //                 num += target.GetEffectStacks(STATUS_EFFECT.FATIGUE);
    //             }
    //         }
    //
    //         if (GlobalEffectHelper.bonusDamageSlow > 0f && entity.HasEffect(STATUS_EFFECT.SLOW))
    //         {
    //             num = Mathf.CeilToInt((float)num * (1f + GlobalEffectHelper.bonusDamageSlow));
    //         }
    //
    //         attack.value = num;
    //         if (GlobalEffectHelper.frontAllyLessDamage != 0 && target.IsPlayer &&
    //             BattleManager.GetFrontAlly(target) == target)
    //         {
    //             num = Mathf.Clamp(num - GlobalEffectHelper.frontAllyLessDamage, 1, 99999);
    //         }
    //         else if (GlobalEffectHelper.frontAllyLessDamageEnemy != 0 && !target.IsPlayer &&
    //                  BattleManager.GetFrontAlly(target) == target)
    //         {
    //             num = Mathf.Clamp(num - GlobalEffectHelper.frontAllyLessDamageEnemy, 1, 99999);
    //         }
    //
    //         if (entity.IsPlayer)
    //         {
    //             StaticQuestUnlocker.AddValue(QUEST_VALUES.DAMAGE_DEALT, num);
    //         }
    //
    //         BattleManager.AddDamageDealt(num, entity);
    //         target?.GetHit(num, damageEffectiveness2, attackType, delegate { CheckPassives(attack, entity, target); },
    //             null, !unblockableDamage, canBlock: true, entity);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void CheckPassives(Attack attack, Entity entity, Entity target)
    // {
    //     if (IsPassive)
    //     {
    //         return;
    //     }
    //
    //     StatusEffectHelper.OnThorns(entity, target);
    //     if (KeywordController.UnitHasKeyword(entity, KEYWORDS.PIERCE))
    //     {
    //         Entity behind = BattleManager.GetBehind(target);
    //         if (behind != null && behind != target)
    //         {
    //             behind.GetHit(attack.value, DamageType.Normal, TYPE.NONE, null, null, checkPassive: true,
    //                 canBlock: true, entity);
    //         }
    //     }
    //
    //     if (KeywordController.UnitHasKeyword(entity, KEYWORDS.BATTERY))
    //     {
    //         entity.ApplyEffect(STATUS_EFFECT.CHARGE, 1, null, unpause: false);
    //     }
    //
    //     if (target.HasEffect(STATUS_EFFECT.FRAIL))
    //     {
    //         int effectStacks = target.GetEffectStacks(STATUS_EFFECT.FRAIL);
    //         target.ConsumeEffect(STATUS_EFFECT.FRAIL, effectStacks);
    //     }
    //
    //     if (target.HasEffect(STATUS_EFFECT.TAUNT))
    //     {
    //         target.ConsumeEffect(STATUS_EFFECT.TAUNT, 1);
    //     }
    // }
    //
    // private void ApplyEffects(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //     }
    //     else
    //     {
    //         if (applyToHit.Count <= 0)
    //         {
    //             return;
    //         }
    //
    //         attack.elementType = TYPE.BASE_GAME;
    //         foreach (StatusEffectStack item in applyToHit)
    //         {
    //             target.ApplyEffect(item.effectType, item.stacks, attacker, unpause: true, CurrentAction);
    //             CurrentAction++;
    //         }
    //     }
    // }
    //
    // private void TriggerDecay(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     for (int i = 0; i < attack.value; i++)
    //     {
    //         BattleManager.TriggerDecayOnTarget(target, attacker);
    //     }
    //
    //     BattleManager.FinishAction();
    // }
    //
    // private void GiveEnergy(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //     }
    //     else
    //     {
    //         DoAttack();
    //     }
    //
    //     void DoAttack()
    //     {
    //         attack.elementType = TYPE.BASE_GAME;
    //         attack.Projectile = StatusEffectHelper.GetEnergyAttack();
    //         Entity entity = attacker;
    //         Entity target2 = target;
    //         ATTACK_ANIM anim = attack.anim;
    //         Attack attack2 = attack;
    //         Action onHit = OnHit;
    //         int delayTimes = CurrentAction;
    //         entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //             delayTimes);
    //         delayTimes = CurrentAction;
    //         CurrentAction = delayTimes + 1;
    //     }
    //
    //     void OnHit()
    //     {
    //         target.AddEnergy(attack.value, wasEffect: true);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void TriggerSuper(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if ((target == null || !target.IsAlive) && !AllowInDeath)
    //     {
    //         BattleManager.FinishAction();
    //     }
    //     else
    //     {
    //         DoAttack();
    //     }
    //
    //     void DoAttack()
    //     {
    //         attack.Projectile = StatusEffectHelper.GetSuperAttack();
    //         attack.elementType = TYPE.BASE_GAME;
    //         for (int i = 0; i < attack.value; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         target.ForceSuper(1);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void GainStartEffect(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         attack.Projectile = StatusEffectHelper.GetSuperAttack();
    //         attack.elementType = TYPE.BASE_GAME;
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         BonusStats bonus = new BonusStats
    //         {
    //             amount = attack.Value2,
    //             stat = (UNIT_STATS)attack.value
    //         };
    //         BoxUnits.AlreadyHas(target.unitScriptable)?.AddBonusStats(bonus);
    //     }
    // }
    //
    // private void TriggerAttack(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetNormalAttack();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < attack.value; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         target.ForceAttack(1);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void ReduceShield(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f, delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         target?.ReduceShieldPercent(attack.value);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void HealUnits(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.Projectile = StatusEffectHelper.GetHeal();
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity entity2 = entity;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity2.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //         delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         BattleManager.AddHealing(attack.value, entity);
    //         target?.GetHeal(attack.value, TYPE.GRASS);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void BonusHealthUnits(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.Projectile = StatusEffectHelper.GetHeal();
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity entity2 = entity;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity2.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //         delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         BattleManager.AddHealing(attack.value, entity);
    //         target?.GetBonusHealth(attack.value, TYPE.GRASS);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void GrantEXP(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target != null)
    //     {
    //         BattleManager.AddExpToUnit(target.unitScriptable, attack.value);
    //     }
    // }
    //
    // private void MoveSelf(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.Projectile = StatusEffectHelper.GetMoveAttack();
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f, delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         BattleManager.PushEntityBack(target, attack.value);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void PushBack(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     int moveAmount = attack.value;
    //     if (attack.attackEffect == ATTACK_EFFECT.MOVE_TO_RANDOM)
    //     {
    //         moveAmount = UnityEngine.Random.Range(-3, 4);
    //     }
    //
    //     attack.Projectile = StatusEffectHelper.GetMoveAttack();
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack, onHit, null, isPassive: false, 30, 1f, delayTimes);
    //     CurrentAction++;
    //
    //     void DoMove()
    //     {
    //         BattleManager.PushEntityBack(target, moveAmount);
    //         BattleManager.FinishAction();
    //     }
    //
    //     void OnHit()
    //     {
    //         DoMove();
    //     }
    // }
    //
    // private void SummonUnit(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     PSHLogging.Log("Summon Unit");
    //     for (int i = 0; i < attack.value; i++)
    //     {
    //         BattleManager.SpawnToken(attack.toSummon, front: true, entity?.IsPlayer ?? true, attack.Value2,
    //             attack.Value3, entity.unitScriptable);
    //         CurrentAction++;
    //     }
    //
    //     PSHLogging.Log("Summon unit complete");
    //     entity.onSummon?.Invoke();
    //     BattleManager.FinishAction();
    // }
    //
    // private void SummonAllBoxUnit(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     PSHLogging.Log("Attempting to spawn box Unit mon");
    //     List<UnitScriptable> list = new List<UnitScriptable>();
    //     UnitScriptable[] playerBoxUnits = MonInventory.playerBoxUnits;
    //     foreach (UnitScriptable unitScriptable in playerBoxUnits)
    //     {
    //         if (!(unitScriptable == null))
    //         {
    //             list.Add(unitScriptable);
    //         }
    //     }
    //
    //     for (int j = 0; j < list.Count; j++)
    //     {
    //         if (!(list[j] == null))
    //         {
    //             UnitScriptable unitScriptable2 = BattleManager.NewUnit(list[j]);
    //             unitScriptable2.ForceTypes(new List<TYPE>
    //             {
    //                 TYPE.CHAOS,
    //                 TYPE.CHAOS
    //             });
    //             BattleManager.SpawnToken(unitScriptable2, front: true, entity?.IsPlayer ?? true, -1, 2,
    //                 entity.unitScriptable);
    //             CurrentAction++;
    //         }
    //     }
    //
    //     PSHLogging.Log("Box units spawned");
    //     entity.onSummon?.Invoke();
    //     BattleManager.FinishAction();
    // }
    //
    // private void SpawnAllReleased(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     PSHLogging.Log("Attempting to spawn released mon");
    //     int count = BattleManager.allReleasedMon.Count;
    //     count = Mathf.Clamp(count, 0, 10);
    //     for (int i = 0; i < count; i++)
    //     {
    //         if (!(BattleManager.allReleasedMon[i] == null))
    //         {
    //             UnitScriptable unitScriptable = BattleManager.NewUnit(BattleManager.allReleasedMon[i]);
    //             unitScriptable.ForceTypes(new List<TYPE>
    //             {
    //                 TYPE.CHAOS,
    //                 TYPE.CHAOS
    //             });
    //             BattleManager.SpawnToken(unitScriptable, front: true, entity?.IsPlayer ?? true, -1, 1,
    //                 entity.unitScriptable);
    //             CurrentAction++;
    //         }
    //     }
    //
    //     PSHLogging.Log("Released Mon Spawned");
    //     BattleManager.allReleasedMon.Clear();
    //     entity.onSummon?.Invoke();
    //     BattleManager.FinishAction();
    // }
    //
    // private void SummonRandomBoxUnit(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     for (int i = 0; i < attack.value; i++)
    //     {
    //         UnitScriptable randomBoxUnitCopy = MonInventory.GetRandomBoxUnitCopy();
    //         if (randomBoxUnitCopy != null)
    //         {
    //             BattleManager.SpawnToken(randomBoxUnitCopy, front: true, entity?.IsPlayer ?? true, attack.Value2,
    //                 attack.Value3, entity.unitScriptable);
    //             CurrentAction++;
    //         }
    //     }
    //
    //     entity.onSummon?.Invoke();
    //     BattleManager.FinishAction();
    // }
    //
    // private void SummonSelfLowerLevel(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (entity.unitScriptable.Level > 0)
    //     {
    //         BattleManager.SpawnToken(entity.unitScriptable, front: true, entity?.IsPlayer ?? true, -1,
    //             entity.unitScriptable.Level - 1, entity.unitScriptable);
    //         CurrentAction++;
    //         entity.onSummon?.Invoke();
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void EatUnit(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (!canTargetSelf && entity == target)
    //     {
    //         return;
    //     }
    //
    //     BattleManager.AddAction(delegate
    //     {
    //         Attack attack2 = new Attack(attack)
    //         {
    //             anim = ATTACK_ANIM.THROW,
    //             elementType = TYPE.BASE_GAME
    //         };
    //         entity.AttackTargetVisual(target, TYPE.NONE, attack2.anim, attack2, delegate
    //         {
    //             entity.unitScriptable.storedUnit = target.unitScriptable;
    //             entity.onEatUnit?.Invoke();
    //             target.GetEaten();
    //             target.GetHit(1, DamageType.Normal, TYPE.NONE, null, null, checkPassive: false, canBlock: false);
    //         });
    //     }, new List<Entity> { BattleManager.GetFrontAlly(entity) }, entity, MyEffectSpeed);
    // }
    //
    // private void SummonEnemyUnit(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     for (int i = 0; i < attack.value; i++)
    //     {
    //         BattleManager.SpawnToken(attack.toSummon, front: true, !entity.IsPlayer, -1, attack.Value3,
    //             entity.unitScriptable);
    //         CurrentAction++;
    //     }
    //
    //     BattleManager.FinishAction();
    // }
    //
    // private void GrantGold(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.colorOverride = BattleManager.unitsManager.unitDictionary.miscColours["HEALTH"];
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target, TYPE.BASE_GAME, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //         delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         ShopItemControllers.AddGold(attack.value, null);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void PlayerHealth(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.colorOverride = BattleManager.unitsManager.unitDictionary.miscColours["HEALTH"];
    //     ATTACK_ANIM anim = attack.anim;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target, TYPE.BASE_GAME, anim, attack, onHit, null, isPassive: false, 30, 1f,
    //         delayTimes);
    //     CurrentAction++;
    //
    //     static void OnHit()
    //     {
    //         PlayerHealthController.Instance.SpawnDamageEffect(1, new Vector3(0f, 1f, 0f));
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void ShieldUnits(Attack attack, Entity entity, Entity target, bool skipWaiting)
    // {
    //     if (target == null || !target.IsAlive || !entity.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.Projectile = StatusEffectHelper.GetShield();
    //     attack.elementType = TYPE.BASE_GAME;
    //     Entity target2 = target;
    //     ATTACK_ANIM anim = attack.anim;
    //     Attack attack2 = attack;
    //     Action onHit = OnHit;
    //     int delayTimes = CurrentAction;
    //     entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f, delayTimes);
    //     CurrentAction++;
    //
    //     void OnHit()
    //     {
    //         target?.GetShield(attack.value);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void TriggerOnAttack(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetPassive();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         PassiveTriggerHelper.BeforeAttack(target, fakeAttack: true);
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void TriggerOnHit(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetPassive();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < attack.value; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         PassiveTriggerHelper.OnHurt(target, BattleManager.GetRandomEnemy(target));
    //         BattleManager.FinishAction();
    //     }
    // }
    //
    // private void TriggerDizzy(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     if (target == null || !target.IsAlive)
    //     {
    //         BattleManager.FinishAction();
    //         return;
    //     }
    //
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetPassive();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < attack.value; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         StatusEffectHelper.OnDizzy(target);
    //     }
    // }
    //
    // private void EnemyEffect(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetPassive();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < attack.value; i++)
    //         {
    //             Entity entity = target;
    //             Entity target2 = attacker;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         target.unitScriptable.GetEffectPassive()?.CheckOnEnergy(attacker, allowDeath: false, attack.Value2);
    //     }
    // }
    //
    // private void CopyAndTriggerEffect(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetNormalAttack();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = target;
    //             Entity target2 = attacker;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         target.unitScriptable.GetEffectPassive()?.CheckOnEnergy(attacker, allowDeath: false, attack.Value2);
    //     }
    // }
    //
    // private void DecayToRegen(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetSuperAttack();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         int effectStacks = target.GetEffectStacks(STATUS_EFFECT.DECAY);
    //         if (effectStacks > 0)
    //         {
    //             target.RemoveEffect(STATUS_EFFECT.DECAY);
    //             target.ApplyEffect(STATUS_EFFECT.REGEN, effectStacks, target, unpause: true, CurrentAction);
    //         }
    //     }
    // }
    //
    // private void SlowToStrength(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetSuperAttack();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         int effectStacks = target.GetEffectStacks(STATUS_EFFECT.SLOW);
    //         if (effectStacks > 0)
    //         {
    //             target.RemoveEffect(STATUS_EFFECT.SLOW);
    //             target.ApplyEffect(STATUS_EFFECT.STRENGTH, effectStacks, target, unpause: true, CurrentAction);
    //         }
    //     }
    // }
    //
    // private void DamageEqualToDecay(Attack attack, Entity attacker, Entity target, bool skipWaiting = false)
    // {
    //     attack.elementType = TYPE.BASE_GAME;
    //     attack.Projectile = StatusEffectHelper.GetDamage();
    //     DoAttack();
    //
    //     void DoAttack()
    //     {
    //         for (int i = 0; i < 1; i++)
    //         {
    //             Entity entity = attacker;
    //             Entity target2 = target;
    //             ATTACK_ANIM anim = attack.anim;
    //             Attack attack2 = attack;
    //             Action onHit = OnHit;
    //             int delayTimes = CurrentAction;
    //             entity.AttackTargetVisual(target2, TYPE.NONE, anim, attack2, onHit, null, isPassive: false, 30, 1f,
    //                 delayTimes);
    //             delayTimes = CurrentAction;
    //             CurrentAction = delayTimes + 1;
    //         }
    //     }
    //
    //     void OnHit()
    //     {
    //         int num = target.GetEffectStacks(STATUS_EFFECT.DECAY);
    //         if (GlobalEffectHelper.bonusDamageSlow > 0f && target.HasEffect(STATUS_EFFECT.SLOW))
    //         {
    //             num = Mathf.CeilToInt((float)num * (1f + GlobalEffectHelper.bonusDamageSlow));
    //         }
    //
    //         if (num > 0)
    //         {
    //             target.GetHit(num, DamageType.Normal, TYPE.NONE);
    //         }
    //     }
    // }
    //
    // private static bool DrawColoredEnumElement(Rect rect, bool value)
    // {
    //     if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
    //     {
    //         value = !value;
    //         GUI.changed = true;
    //         Event.current.Use();
    //     }
    //
    //     return value;
    // }
}