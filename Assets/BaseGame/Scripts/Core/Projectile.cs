using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using LitMotion;
using TW.Utility.CustomComponent;
using UnityEngine;
using Zenject;
using Ease = LitMotion.Ease;
using Object = UnityEngine.Object;

public delegate UniTask HeroToHeroCallback(Hero fromHero, Hero toHero, TickRate tickRate, CancellationToken cancellationToken);
public delegate UniTask HeroToHolderCallback(Hero fromHero, Holder toHolder, TickRate tickRate, CancellationToken cancellationToken);
public class Projectile : ACachedMonoBehaviour
{
    [field: SerializeField] private Transform Shadow {get; set;}
    [field: SerializeField] public AnimationCurve JumpCurve {get; private set;}
    [field: SerializeField] public AnimationCurve ScaleCurve {get; private set;}
    [field: SerializeField] public AnimationCurve ShadowScaleCurve {get; private set;}
    public class Factory : PlaceholderFactory<Object, Projectile>
    {
        public static Factory CreateInstance()
        {
            return new Factory();
        }
    }
    
    public async UniTask Move(Hero fromHero, Hero toHero, HeroToHeroCallback onComplete, TickRate tickRate, CancellationToken cancellationToken)
    {
        Vector3 from = fromHero.Transform.position;
        Vector3 to = toHero.Transform.position;
        Vector3 axis = Vector3.up;
        float duration =  UnityEngine.Random.Range(0.3f, 0.5f)/ tickRate.ToValue();
        await LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.Linear)
            .WithOnComplete(SelfDespawn)
            .Bind(UpdatePosition)
            .ToUniTask(cancellationToken: cancellationToken);
        
        await onComplete.Invoke(fromHero, toHero, tickRate, cancellationToken);
        
        return;
        void UpdatePosition(float value)
        {
            Transform.localScale = Vector3.one * ScaleCurve.Evaluate(value);
            Vector3 position = Vector3.Lerp(from, to, value);
            Transform.position = position + JumpCurve.Evaluate(value) * axis.normalized * 3f;
            Shadow.position = position;
            Shadow.localScale = Vector3.one * ShadowScaleCurve.Evaluate(value);
        }
    }
    public async UniTask Move(Hero fromHero, Holder toHolder, HeroToHolderCallback onComplete, TickRate tickRate, CancellationToken cancellationToken)
    {
        Vector3 from = fromHero.Transform.position;
        Vector3 to = toHolder.Transform.position;
        Vector3 axis = Vector3.up;
        float duration =  UnityEngine.Random.Range(0.3f, 0.5f)/ tickRate.ToValue();
        await LMotion.Create(0f, 1f, duration)
            .WithEase(Ease.Linear)
            .WithOnComplete(SelfDespawn)
            .Bind(UpdatePosition)
            .ToUniTask(cancellationToken: cancellationToken);
        
        await onComplete.Invoke(fromHero, toHolder, tickRate, cancellationToken);
        
        return;
        void UpdatePosition(float value)
        {
            Transform.localScale = Vector3.one * ScaleCurve.Evaluate(value);
            Vector3 position = Vector3.Lerp(from, to, value);
            Transform.position = position + JumpCurve.Evaluate(value) * axis.normalized * 3f;
            Shadow.position = position;
            Shadow.localScale = Vector3.one * ShadowScaleCurve.Evaluate(value);
        }
    }

    private void SelfDespawn()
    {
        Destroy(gameObject);
    }
}