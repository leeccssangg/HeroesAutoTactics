using System;
using System.Collections.Generic;
using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Game.Manager
{

    public class AbilityResolveManager : Singleton<AbilityResolveManager>
    {
        public class AbilityResolveElement
        {
            public Hero Hero { get; set; }
            public AbilityPower AbilityPower { get; set; }
            public TickRate TickRate { get; set; }
            public CancellationToken CancellationToken { get; set; }

            public AbilityResolveElement(Hero hero, AbilityPower abilityPower, TickRate tickRate, CancellationToken cancellationToken)
            {
                Hero = hero;
                AbilityPower = abilityPower;
                TickRate = tickRate;
                CancellationToken = cancellationToken;
            }
            public async UniTask Resolve()
            {
                Debug.Log($"{Hero.HeroConfigData.HeroName} Resolve {AbilityPower.Description}");
                await AbilityPower.Resolve(Hero, TickRate, CancellationToken);
            }
        }
        private Queue<AbilityResolveElement> AbilityPowerResolveQueue { get; set; } = new();
        
        private bool IsResolving { get; set; }
        private bool BlockResolve { get; set; }
        public Func<bool> NotBlockResolveFunc { get; set; }
        public Func<bool> IsResolvingCompleteFunc { get; set; }
        protected override void Awake()
        {
            base.Awake();
            NotBlockResolveFunc = NotBlockResolve;
            IsResolvingCompleteFunc = IsResolvingComplete;
        }

        public void SetBlockResolve(bool blockResolve)
        {
            BlockResolve = blockResolve;
        }
        public void Resolve(Hero hero, AbilityPower abilityPower, TickRate tickRate, CancellationToken cancellationToken)
        {
            AbilityPowerResolveQueue.Enqueue(new AbilityResolveElement(hero, abilityPower, tickRate, cancellationToken));
            StartResolve().Forget();
        }


        private async UniTask StartResolve()
        {
            if (IsResolving) return;
            IsResolving = true;
            while (AbilityPowerResolveQueue.Count != 0)
            {
                await UniTask.WaitUntil(NotBlockResolveFunc);
                await AbilityPowerResolveQueue.Dequeue().Resolve();
            }

            IsResolving = false;
        }

        public bool IsResolvingComplete()
        {
            return AbilityPowerResolveQueue.Count == 0 && !IsResolving;
        }

        private bool NotBlockResolve()
        {
            return !BlockResolve;
        }
    }
}