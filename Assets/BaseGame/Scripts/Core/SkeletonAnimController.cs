using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkeletonAnimController : MonoBehaviour
{
    private static readonly string IdleAnim = "idle";
    private static readonly string AttackAnim = "attack";
    private static readonly string FaintAnim = "die";
    [field: SerializeField] public SkeletonAnimation SkeletonAnimation { get; private set; }
    [field: SerializeField] private float IdleAnimDuration { get; set; }
    [field: SerializeField] private float AttackAnimDuration { get; set; }
    [field: SerializeField] private float HitAnimDuration { get; set; }
    [field: SerializeField] private float FaintAnimDuration { get; set; }
    private bool IsAttacking { get; set; }

    public void SetFlip(bool isFlip)
    {
        SkeletonAnimation.transform.rotation = Quaternion.Euler(0, isFlip ? 180 : 0, 0);
    }

    public void EditorSetup()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        try
        {
            IdleAnimDuration = SkeletonAnimation.Skeleton.Data.FindAnimation(IdleAnim).Duration;
            AttackAnimDuration = SkeletonAnimation.Skeleton.Data.FindAnimation(AttackAnim).Duration;
            FaintAnimDuration = SkeletonAnimation.Skeleton.Data.FindAnimation(FaintAnim).Duration;
        }
        catch (Exception e)
        {
            Debug.Log(e);  
        }

    }

    private void Start()
    {
        PlayAnimation(IdleAnim, true, TickRate.Normal);
    }

    private void PlayAnimation(string animationName, bool loop, TickRate tickRate)
    {
        if (SkeletonAnimation.skeletonDataAsset == null) return;
        SkeletonAnimation.timeScale = tickRate.ToValue();
        SkeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }

    [Button]
    public async UniTask Attack(TickRate tickRate, CancellationToken cancellationToken)
    {
        IsAttacking = true;
        PlayAnimation(AttackAnim, false, tickRate);
        await UniTask.Delay((int)(AttackAnimDuration * 1000 / tickRate.ToValue()),
            cancellationToken: cancellationToken);
        PlayAnimation(IdleAnim, true, tickRate);
        IsAttacking = false;
    }

    [Button]
    public async UniTask Hit(TickRate tickRate, CancellationToken cancellationToken)
    {
        await UniTask.Delay((int)(HitAnimDuration * 1000 / tickRate.ToValue()), cancellationToken: cancellationToken);
    }

    [Button]
    public async UniTask Faint(TickRate tickRate, CancellationToken cancellationToken)
    {
        await UniTask.WaitUntil(() => !IsAttacking, cancellationToken: cancellationToken);
        PlayAnimation(FaintAnim, false, tickRate);
        await UniTask.Delay((int)(FaintAnimDuration * 1000 / tickRate.ToValue()), cancellationToken: cancellationToken);
    }
}