using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public abstract partial class Holder : ACachedMonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler
    {
        [field: Title(nameof(Holder))]
        [field: SerializeField] public bool CanDragIn { get; set; }

        [field: SerializeField] public Hero OwnerHero { get; private set; }
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
        public abstract void OnPointerClick(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnBeginDrag(PointerEventData eventData);
        public abstract void OnDrag(PointerEventData eventData);
        public abstract void OnEndDrag(PointerEventData eventData);
        private MotionHandle ForceAddMotion { get; set; }
        private Holder[] ForceAddMotionPoints { get; set; }
        public bool HasHero => OwnerHero != null;
        public void AddHeroImmediate(Hero hero)
        {
            if (TryUpgradeHeroImmediate(hero)) return;
            InitNewHeroImmediate(hero);
        }

        protected void InitNewHeroImmediate(Hero hero)
        {
            OwnerHero = hero;
            OwnerHero.SetHolder(this);
            hero.SetHeroInstancePosition(Transform.position);
        }

        private bool TryUpgradeHeroImmediate(Hero hero)
        {
            if (!HasHero) return false;
            if (OwnerHero.HeroConfigData != hero.HeroConfigData) return false;
            if (OwnerHero.IsMaxStar()) return false;
            OwnerHero.Fusion(hero);
            hero.SetSelfDespawnAfterDrag(true);
            hero.SetHeroStarProcess(false);
            
            hero.SetHeroInstancePosition(Transform.position);
            return true;
        }

        public void AddHero(Hero hero)
        {
            if (TryUpgradeHero(hero)) return;
            InitNewHero(hero);
        }

        public void InitNewHero(Hero hero)
        {
            OwnerHero = hero;
            OwnerHero.SetHolder(this);
            hero.Transform.position = Transform.position;
            hero.SetHeroTargetPosition(Transform.position);
        }

        private bool TryUpgradeHero(Hero hero)
        {
            if (!HasHero) return false;
            if (OwnerHero.HeroConfigData != hero.HeroConfigData) return false;
            if (OwnerHero.IsMaxStar()) return false;
            OwnerHero.Fusion(hero);
            hero.SetSelfDespawnAfterDrag(true);
            hero.SetHeroStarProcess(false);
            
            hero.Transform.position = Transform.position;
            hero.SetHeroTargetPosition(Transform.position);
            return true;
        }

        public void ForceAddHeroInstance(Hero hero)
        {
            OwnerHero = hero;
            OwnerHero.SetHolder(this);
            hero.SetHeroInstancePosition(Transform.position);
        }
        public void ForceAddHero(Hero hero)
        {
            OwnerHero = hero;
            OwnerHero.SetHolder(this);
            ForceAddMotion.TryCancel();
            ForceAddMotion = LMotion.Create(hero.Transform.position, Transform.position, 0.2f)
                .WithEase(Ease.OutSine)
                .Bind(OnForceAddMotionUpdatePositionCache);
        }
        public async UniTask ForceAddHero(Holder[] holderPoint,  Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            OwnerHero = hero;
            OwnerHero.SetHolder(this);
            float duration = 0.2f / tickRate.ToValue() * (holderPoint.Length - 1); 
            
            ForceAddMotionPoints = holderPoint;
            ForceAddMotion.TryCancel();
            ForceAddMotion = LMotion.Create(0f, holderPoint.Length - 1, duration)
                .WithEase(Ease.OutSine)
                .WithOnComplete(OnForceAddMotionCompleteCache)
                .Bind(OnForceAddMotionUpdateFollowPointCache);
            await ForceAddMotion.ToUniTask(cancellationToken);
        }
        
        [ACacheMethod]
        private void OnForceAddMotionUpdatePosition(UnityEngine.Vector3 value)
        {
            OwnerHero.Transform.position = value;
        }
        [ACacheMethod]
        private void OnForceAddMotionUpdateFollowPoint(float value)
        {
            int start = Mathf.FloorToInt(value);
            int end = Mathf.CeilToInt(value);
            float alpha = value - start;
            Vector3 startPoint = ForceAddMotionPoints[start].Transform.position;
            Vector3 endPoint = ForceAddMotionPoints[end].Transform.position;
            OwnerHero.Transform.position = Vector3.Lerp(startPoint, endPoint, alpha);
        }
        [ACacheMethod]
        private void OnForceAddMotionComplete()
        {
            OwnerHero.SetHeroInstancePosition(Transform.position);
        }

        public void RemoveHero()
        {
            OwnerHero = null;
        }
        public void RemoveHero(Hero hero)
        {
            if (OwnerHero != hero) return;
            OwnerHero = null;
        }
    }
    
    public static class HolderExtension
    {
        public static Holder[] Shuffle(this Holder[] holders ,System.Random random)
        {
            int n = holders.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (holders[k], holders[n]) = (holders[n], holders[k]);
            }
            return holders;
        }
        public static Holder[] ToHolder(this BattleSlot[] battleSlots)
        {
            Holder[] holders = new Holder[battleSlots.Length];
            for (int i = 0; i < battleSlots.Length; i++)
            {
                holders[i] = battleSlots[i];
            }
            return holders;
        }
        public static Holder[] ToHolder(this FieldSlot[] fieldSlots)
        {
            Holder[] holders = new Holder[fieldSlots.Length];
            for (int i = 0; i < fieldSlots.Length; i++)
            {
                holders[i] = fieldSlots[i];
            }
            return holders;
        }
        public static Holder[] ToHolder(this StoreSlot[] storeSlots)
        {
            Holder[] holders = new Holder[storeSlots.Length];
            for (int i = 0; i < storeSlots.Length; i++)
            {
                holders[i] = storeSlots[i];
            }
            return holders;
        }
    }
}