using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Game.Manager
{
    public class InGameSleepState : SingletonState<InGameHandleManager, InGameSleepState>
    {
    }

    public partial class InGameHandleManager : InGameSleepState.IHandler
    {
        public UniTask OnStateEnter(InGameSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExecute(InGameSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(InGameSleepState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}