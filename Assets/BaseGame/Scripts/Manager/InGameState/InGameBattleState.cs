using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using Game.DataAccess;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using Random = System.Random;

namespace Game.Manager
{
    public class InGameBattleState : SingletonState<InGameHandleManager, InGameBattleState>
    {
    }

    public partial class InGameHandleManager : InGameBattleState.IHandler
    {
        public async UniTask OnStateEnter(InGameBattleState state, CancellationToken token)
        {
            JsonDataList.Add(new BattleTestData(Heart, Round, Win, PlayerBattleData, OpponentBattleData).ToJsonData());
            ClearAllFieldSlot();
            
            ScreenOptions options = new ScreenOptions(nameof(ScreenBattle), stack: false);
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options, PlayerBattleData, OpponentBattleData);
            
            BattleGroup.SetActive(true);
            PrepareGroup.SetActive(false);
            
            PlayerTeam.InitBattleTeam(PlayerBattleData.InGameTeamData);
            OpponentTeam.InitBattleTeam(OpponentBattleData.InGameTeamData);
            RandomSeed = PlayerTeam.InGameTeamData.TeamSeed + OpponentTeam.InGameTeamData.TeamSeed;
            
            BattleTeamArray = RandomSeed % 2 == 0
                ? new[] { OpponentTeam, PlayerTeam }
                : new[] { PlayerTeam, OpponentTeam };
            CurrentTeam = RandomSeed % 2;
            Random = new Random(RandomSeed);
            
            await UniTask.Delay(500, cancellationToken:token);
            AbilityResolveManager.SetBlockResolve(true);
            for (int i = 0; i < BattleTeamArray[0].BattleSlotArray.Length; i++)
            {
                if (BattleTeamArray[0].BattleSlotArray[i].TryGetHero(out Hero hero1))
                {
                    hero1.TryTriggerAbility<SelfStartOfBattleTrigger>(TickRate, token);
                }
                if (BattleTeamArray[1].BattleSlotArray[i].TryGetHero(out Hero hero2))
                {
                    hero2.TryTriggerAbility<SelfStartOfBattleTrigger>(TickRate, token);
                }
            }
            AbilityResolveManager.SetBlockResolve(false);
            await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: token);
        }

        public async UniTask OnStateExecute(InGameBattleState state, CancellationToken token)
        {
            await UniTask.Delay(500, cancellationToken:token);
            await UniTask.WhenAll(PlayerTeam.ReRangeHero(TickRate, token), OpponentTeam.ReRangeHero(TickRate, token));
            while (BattleTeamArray[0].TryGetFirstHero(out Hero hero1) && BattleTeamArray[1].TryGetFirstHero(out Hero hero2))
            {
                AbilityResolveManager.SetBlockResolve(true);
                hero1.TryTriggerAbility<SelfBeforeAttackTrigger>(TickRate, token);
                hero2.TryTriggerAbility<SelfBeforeAttackTrigger>(TickRate, token);
                AbilityResolveManager.SetBlockResolve(false);
                await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: token);

                AbilityResolveManager.SetBlockResolve(true);
                await UniTask.WhenAll(hero1.Attack(hero2, TickRate, token), hero2.Attack(hero1, TickRate, token));
                AbilityResolveManager.SetBlockResolve(false);
                await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: token);
                
                // EffectResolveManager.SetBlockResolve(true);
                // await UniTask.WhenAll(BattleTeamArray[0].CheckHealthPoint(TickRate, token), BattleTeamArray[1].CheckHealthPoint(TickRate, token));
                // EffectResolveManager.SetBlockResolve(false);
                // await UniTask.WaitUntil(EffectResolveManager.IsResolvingComplete, cancellationToken: token);
                
            }
            if (PlayerTeam.IsAllFainted() && OpponentTeam.IsAllFainted())
            {
                await UniTask.Delay(500, cancellationToken:token);
                Debug.Log("Draw");
                ViewOptions options = new ViewOptions(nameof(ModalDraw));
                ModalContainer.Find(ContainerKey.OverlayModal).PushAsync(options, Win.Value, Heart.Value);
                // ClearAllBattleSlot();
                StateMachine.RequestTransition(InGameSleepState.Instance);
                return;
            }
            
            if (PlayerTeam.IsAllFainted())
            {
                await UniTask.Delay(500, cancellationToken:token);
                Debug.Log("Opponent Win");
                Heart.Value -= 1;
                ViewOptions options = new ViewOptions(nameof(ModalLose));
                ModalContainer.Find(ContainerKey.OverlayModal).PushAsync(options, Win.Value, Heart.Value);
                // ClearAllBattleSlot();
                StateMachine.RequestTransition(InGameSleepState.Instance);
                return;
            }
            
            if (OpponentTeam.IsAllFainted())
            {
                await UniTask.Delay(500, cancellationToken:token);
                Debug.Log("Player Win");
                Win.Value += 1;
                ViewOptions options = new ViewOptions(nameof(ModalWin));
                ModalContainer.Find(ContainerKey.OverlayModal).PushAsync(options, Win.Value, Heart.Value);
                // ClearAllBattleSlot();
                StateMachine.RequestTransition(InGameSleepState.Instance);
                return;
            }
        }

        public UniTask OnStateExit(InGameBattleState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}