using System.Collections.Generic;
using Game.Core;
using Game.Data;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Game.Manager
{
    public class FieldSlotManager : Singleton<FieldSlotManager>
    {
        [Inject] private InputManager InputManager { get; set; }
        [field: SerializeField] private GameObject Root { get; set; }
        [field: SerializeField] private FieldSlot[] FieldSlotArray { get; set; }
        [field: SerializeField] public List<FieldSlot> Solution { get; private set; }
        [field: SerializeField] public List<FieldSlot> Pool { get; private set; }
        [field: SerializeField] public List<FieldSlot> Mark { get; private set; }

        public void SetActive(bool active)
        {
            Root.SetActive(active);
        }

        public bool IsActive()
        {
            return Root.activeSelf;
        }

        [Button]
        public InGameTeamData GetInGameTeamData()
        {
            return new InGameTeamData(FieldSlotArray);
        }

        private void ForceMoveHeroToEmptyField(Holder fromHeroHolder, Holder toHeroHolder, Hero hero)
        {
            if (toHeroHolder.HasHero) return;
            toHeroHolder.ForceAddHeroInstance(hero);
            fromHeroHolder.RemoveHero();
        }

        public void TryEmptyFieldSlot(FieldSlot fieldSlot)
        {
            Mark = new List<FieldSlot>() { null };
            Pool = new List<FieldSlot>() { fieldSlot };
            Solution = new List<FieldSlot>();
            FindSolution(0, Pool, Mark, Solution);
            if (Solution.Count == 0) return;
            foreach (FieldSlot heroSlot in Solution)
            {
                if (heroSlot == InputManager.CurrentHolderDrag)
                {
                    InputManager.SetCurrentHolderDrag(fieldSlot);
                }
            }

            for (int i = 0; i < Solution.Count - 1; i++)
            {
                ForceMoveHeroToEmptyField(Solution[i + 1], Solution[i], Solution[i + 1].OwnerHero);
            }
        }

        private void FindSolution(int currentStep, List<FieldSlot> pool, List<FieldSlot> mark,
            List<FieldSlot> solution)
        {
            if (currentStep >= pool.Count)
            {
                return;
            }

            FieldSlot fieldSlot = pool[currentStep];
            if (!fieldSlot.HasHero)
            {
                GetSolution(pool, mark, solution, fieldSlot);
                return;
            }

            foreach (FieldSlot slotAround in fieldSlot.FieldSlotAround)
            {
                if (pool.Contains(slotAround)) continue;
                pool.Add(slotAround);
                mark.Add(fieldSlot);
            }

            FindSolution(currentStep + 1, pool, mark, solution);
        }

        private static void GetSolution(List<FieldSlot> pool, List<FieldSlot> mark, List<FieldSlot> solution,
            FieldSlot fieldSlot)
        {
            solution.Add(fieldSlot);
            while (true)
            {
                int index = pool.IndexOf(solution[^1]);
                if (index == 0) break;
                solution.Add(mark[index]);
            }
        }
    }
}