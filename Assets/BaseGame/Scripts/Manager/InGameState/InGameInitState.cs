using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;

namespace Game.Manager
{
    public class InGameInitState : SingletonState<InGameHandleManager, InGameInitState>
    {
    }

    public partial class InGameHandleManager : InGameInitState.IHandler
    {
        public UniTask OnStateEnter(InGameInitState state, CancellationToken token)
        {
            Heart = new ReactiveValue<int>(5);
            Round = new ReactiveValue<int>(0);
            Win = new ReactiveValue<int>(0);
            Coin = new ReactiveValue<int>(0);
            FreeRollCount = new ReactiveValue<int>(0);
            
            PrepareGroup.SetActive(false);
            BattleGroup.SetActive(false);    
            PlayerBattleData = new BattleData(InGameDataManager.UserData.Rank, Win, Heart, Round, InGameTeamData.Empty);
            
            
            StateMachine.RequestTransition(InGamePrepareState.Instance);
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(InGameInitState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(InGameInitState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}