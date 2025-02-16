using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using LitMotion;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Core
{
    public partial class HeroAnim : ACachedMonoBehaviour
    {
        private delegate void UpdatePosition(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition,
            TickRate tickRate,
            out bool isComplete);

        [field: SerializeField] public Transform GraphicGroup { get; private set; }
        [field: SerializeField] public SortingGroup SortingGroup { get; private set; }
        [field: SerializeField] public SkeletonAnimController SkeletonAnimController { get; private set; }

        [field: Title("Movement")]
        [field: SerializeField]
        public Transform ShadowTransform { get; private set; }

        [field: SerializeField] private float MaxMoveSpeed { get; set; }
        [field: SerializeField] public float TrajectoryMaxHeight { get; private set; }
        [field: SerializeField] public AnimationCurve Trajectory { get; private set; }
        [field: SerializeField] public AnimationCurve Velocity { get; private set; }
        [field: SerializeField] public AnimationCurve Scale { get; private set; }
        [field: SerializeField] public AnimationCurve InScale { get; private set; }
        [field: SerializeField] public AnimationCurve OutScale { get; private set; }
        [field: SerializeField] public AnimationCurve ShadowScale { get; private set; }
        private Vector3 StartPosition { get; set; }
        private float CurrentMoveSpeed { get; set; }
        private AnimationCurve CurrentScale { get; set; }
        


        [field: Title("Attack")]
        [field: SerializeField]
        public float AttackDuration { get; private set; }

        [field: SerializeField] public AnimationCurve AttackPositionCurve { get; private set; }

        [field: Title("Hit")]
        [field: SerializeField]
        public float HitDuration { get; private set; }
        [field: SerializeField] private float MaxAngle { get; set; }
        [field: SerializeField] public AnimationCurve HitBalancedCurve { get; private set; }
        [field: SerializeField] public AnimationCurve HitNoiseCurve { get; private set; }
        [field: SerializeField] public AnimationCurve HitPositionCurve { get; private set; }

        [field: Title("Faint")]
        [field: SerializeField]
        public float FaintDuration { get; private set; }

        [field: Title("Death")]
        [field: SerializeField]
        public float DeathDuration { get; private set; }

        [field: SerializeField] public AnimationCurve DeathScaleCurve { get; private set; }
        private Vector3 RotateAxis { get; set; }

        private MotionHandle MotionHandle { get; set; }

        private Vector3 Direction { get; set; }
        private Vector3 BalancedPosition { get; set; }
        private Vector3 AmplitudePosition { get; set; }
        private bool MotionComplete { get; set; }
        private void OnDestroy()
        {
            MotionHandle.TryCancel();
        }
        public void SetFlip(bool isFlip)
        {
            SkeletonAnimController.SetFlip(isFlip);
        }

        public void SetHeroOrder(int order)
        {
            SortingGroup.sortingOrder = order;
        }


        
        private void StartMotion(float duration, TickRate tickRate,Action<float> onMotionUpdate)
        {
            MotionComplete = false;
            MotionHandle.TryCancel();
            MotionHandle = LMotion.Create(0f, 1f, duration / tickRate.ToValue())
                .WithEase(Ease.Linear)
                .WithOnComplete(OnMotionCompleteCache)
                .Bind(onMotionUpdate);
        }
        [ACacheMethod]
        private bool IsMotionComplete()
        {
            return MotionComplete;
        }
        [ACacheMethod]
        private void OnMotionComplete()
        {
            MotionComplete = true;
        }
        

        private async UniTask StartJumpToPosition(TickRate tickRate,
            CancellationToken cancellationToken)
        {
            CurrentScale = InScale;
            await LMotion.Create(0f, 1f, 2f / MaxMoveSpeed / tickRate.ToValue())
                .WithEase(Ease.Linear)
                .Bind(SetHeroScaleCache)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        private async UniTask EndJumpToPosition(TickRate tickRate,
            CancellationToken cancellationToken)
        {
            CurrentScale = OutScale;
            await LMotion.Create(0f, 1f, 0.5f / MaxMoveSpeed / tickRate.ToValue())
                .WithEase(Ease.Linear)
                .Bind(SetHeroScaleCache)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        public async UniTask JumpToPosition(Vector3 targetPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            StartPosition = Transform.position;
            Vector3 direction = (targetPosition - StartPosition).normalized;
            float distance = Vector3.Distance(targetPosition, StartPosition);
            float alpha = 0;
            SetFlip(direction.x < 0);
            await StartJumpToPosition(tickRate, cancellationToken);
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(cancellationToken))
            {
                float movementDistance = CurrentMoveSpeed * Time.deltaTime * tickRate.ToValue();
                alpha += movementDistance / distance;
                Vector3 nextPosition = Vector3.Lerp(StartPosition, targetPosition, alpha);


                float trajectoryValue = Trajectory.Evaluate(alpha) * TrajectoryMaxHeight * Mathf.Abs(distance);

                Vector3 fixedPosition = Vector3.Lerp(StartPosition, targetPosition, alpha) +
                                        new Vector3(0, trajectoryValue, 0);

                Transform.position = fixedPosition;

                ShadowTransform.position = nextPosition;
                ShadowTransform.localScale = Vector3.one * ShadowScale.Evaluate(alpha);
                
                CurrentScale = Scale;
                SetHeroScale(alpha);

                CurrentMoveSpeed = Velocity.Evaluate(alpha) * MaxMoveSpeed;

                bool isComplete = alpha > 0.99f;
                if (isComplete)
                {
                    break;
                }
            }

            await EndJumpToPosition(tickRate, cancellationToken);
        }
        
        [ACacheMethod]
        private void SetHeroScale(float value)
        {
            Transform.localScale = Vector3.up / CurrentScale.Evaluate(value) + 
                                   Vector3.right * CurrentScale.Evaluate(value) +
                                   Vector3.forward;
        }
        public async UniTask OnAttack(Func<UniTask> onHitImpacted, TickRate tickRate, CancellationToken cancellationToken)
        {
            UniTask anim = SkeletonAnimController.Attack(tickRate, cancellationToken);
            await UniTask.Delay((int)(500 / tickRate.ToValue()), cancellationToken: cancellationToken);
            UniTask onHit = onHitImpacted();

            await UniTask.WhenAll(onHit, anim);
        }

        public async UniTask OnFaint(TickRate tickRate, CancellationToken cancellationToken)
        {
            await SkeletonAnimController.Faint(tickRate, cancellationToken);
            StartMotion(DeathDuration, tickRate, OnFaintUpdateCache);
            await UniTask.WaitUntil(IsMotionCompleteCache, cancellationToken: cancellationToken);
        }
        [ACacheMethod]
        private void OnFaintUpdate(float value)
        {
            Transform.localScale = Vector3.one * DeathScaleCurve.Evaluate(value);
        }
        
        public async UniTask OnImpact(Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            Direction = (endPosition - startPosition).normalized;
            BalancedPosition = Transform.position;
            AmplitudePosition = Transform.position + Direction * 0.5f;
            StartMotion(HitDuration, tickRate, OnImpactUpdateCache);
            await UniTask.WaitUntil(IsMotionCompleteCache, cancellationToken: cancellationToken);
        }
        
        [ACacheMethod]
        private void OnImpactUpdate(float value)
        {
            RotateAxis = Quaternion.Euler(0, 0, -90 + HitNoiseCurve.Evaluate(value)) * Direction;
            Quaternion targetRotation = Quaternion.AngleAxis(MaxAngle, RotateAxis);
            GraphicGroup.rotation =
                Quaternion.LerpUnclamped(targetRotation, Quaternion.identity, HitBalancedCurve.Evaluate(value));
            GraphicGroup.position =
                Vector3.LerpUnclamped(BalancedPosition, AmplitudePosition, HitPositionCurve.Evaluate(value));
        }


        
    }
}