using System.Threading;
using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;
using Random = System.Random;

namespace Game.Manager
{
    public class InGamePrepareState : SingletonState<InGameHandleManager, InGamePrepareState>
    {
    }

    public partial class InGameHandleManager : InGamePrepareState.IHandler
    {
        public async UniTask OnStateEnter(InGamePrepareState state, CancellationToken token)
        {

            Round.Value = Round + 1;
            Coin.Value = 10;
            PrepareGroup.SetActive(true);
            BattleGroup.SetActive(false);
            ClearAllBattleSlot();
            LoadPlayerFieldSlotData();
            RandomSeed = UnityEngine.Random.Range(0, 100);
            Random = new Random(RandomSeed);
            ScreenOptions options = new ScreenOptions(nameof(ScreenPrepare), stack: false);
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }

        public UniTask OnStateExecute(InGamePrepareState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(InGamePrepareState state, CancellationToken token)
        {
            InputManager.InActiveSelect();
            return UniTask.CompletedTask;
        }
    }
}