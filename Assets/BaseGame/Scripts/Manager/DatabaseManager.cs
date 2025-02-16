using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
using Game.Data;
using R3;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;
using Game.DataAccess;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using Random = UnityEngine.Random;


namespace Game.Manager
{
    public class DatabaseManager : Singleton<DatabaseManager>
    {
        private static DatabaseReference RootReference => FirebaseDatabase.DefaultInstance.RootReference;
        private static DatabaseReference BattleReference => RootReference
            .Child(nameof(BattleData));
        private static DatabaseReference UserReference => RootReference
            .Child(nameof(UserData))
            .Child(InGameDataManager.Instance.UserId);
        private static DatabaseReference UserResourceReference => UserReference
            .Child(nameof(UserResourceList));
        [field: SerializeField] public int TimeOutDuration {get; private set;}
        [field: SerializeField] public ReactiveValue<bool> IsOffline {get; private set;}
        private void Start()
        {
            IsOffline.ReactiveProperty.Subscribe(value =>
            {
                if (value)
                {
                    DatabaseReference.GoOffline();
                }
                else
                {
                    DatabaseReference.GoOnline();
                }
            }).AddTo(this);
        }

        #region Get Methods

        public async UniTask<GetUserDataAccess.Response> GetUserData(string userId)
        {
            GetUserDataAccess.Request request = new GetUserDataAccess.Request(userId);
            GetUserDataAccess.Response response = new GetUserDataAccess.Response();
            UniTask<DataSnapshot> task = RootReference.Child(nameof(UserData)).Child(request.UserId).GetValueAsync().AsUniTask();
            DataSnapshot dataSnapshot = await task.RunWithTimeOut(TimeOutDuration);
            if (task.Status.IsFaulted())
            {
                response.Status = DataResponse.ResponseStatus.Faulted;
            } 
            else if (task.Status.IsCompletedSuccessfully())
            {
                response.Status = DataResponse.ResponseStatus.Succeeded;
                response.UserData = new UserData(request.UserId)
                {
                    UserName = dataSnapshot.Child(nameof(UserData.UserName)).GetValue(false).ToString(),
                    Rank = (Rank)Enum.Parse(typeof(Rank), dataSnapshot.Child(nameof(UserData.Rank)).GetValue(false).ToString()),
                    RankPoint = int.Parse(dataSnapshot.Child(nameof(UserData.RankPoint)).GetValue(false).ToString())
                };

                UserResource.Type[] types = (UserResource.Type[])Enum.GetValues(typeof(UserResource.Type));
                foreach (UserResource.Type type in types)
                {
                    UserResource userResource = response.UserData.UserResourceList[type];
                    try
                    {
                        userResource.Amount.Value = int.Parse(dataSnapshot.Child(nameof(UserResourceList)).Child(type.ToString()).GetValue(false).ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"UserResource {type} is not found in database. {e}");
                        userResource.Amount.Value = 0;
                    }
                }
            }
            else
            {
                response.Status = DataResponse.ResponseStatus.TimeOut;
            }
            return response;
        }

        public async UniTask<GetOpponentBattleDataAccess.Response> GetOpponentBattleData(Rank rank, int win, int heart, int round)
        {
            GetOpponentBattleDataAccess.Request request = new GetOpponentBattleDataAccess.Request(rank, win, heart, round);
            GetOpponentBattleDataAccess.Response response = new GetOpponentBattleDataAccess.Response();
            UniTask<DataSnapshot> task = BattleReference
                .Child(request.Rank.ToRoman())
                .Child(request.Win.ToString())
                .Child(request.Heart.ToString())
                .Child(request.Round.ToString())
                .GetValueAsync()
                .AsUniTask();
            DataSnapshot dataSnapshot = await task.RunWithTimeOut(TimeOutDuration);
            if (task.Status.IsFaulted())
            {
                response.Status = DataResponse.ResponseStatus.Faulted;
            }
            else if (task.Status.IsCompletedSuccessfully())
            {
                response.Status = DataResponse.ResponseStatus.Succeeded;
                response.BattleData = new BattleData(request.Rank, request.Win, request.Heart, request.Round, 
                    new InGameTeamData(dataSnapshot.Children.GetRandomElement().GetValue(false).ToString()));
            }
            else
            {
                response.Status = DataResponse.ResponseStatus.TimeOut;
            }
            return response;
        }
        
        #endregion

        #region Post Methods

        public async UniTask<PostPlayerBattleDataAccess.Response> PostPlayerBattleData(BattleData battleData)
        {
            PostPlayerBattleDataAccess.Request request = new PostPlayerBattleDataAccess.Request(battleData);
            PostPlayerBattleDataAccess.Response response = new PostPlayerBattleDataAccess.Response();
            UniTask task = BattleReference
                .Child(request.BattleData.Rank.ToRoman())
                .Child(request.BattleData.Win.ToString())
                .Child(request.BattleData.Heart.ToString())
                .Child(request.BattleData.Round.ToString())
                .Child(Random.Range(0, 20).ToString())
                .SetValueAsync(request.BattleData.TeamValue)
                .AsUniTask();
            await task.RunWithTimeOut(TimeOutDuration);
            if (task.Status.IsFaulted())
            {
                response.Status = DataResponse.ResponseStatus.Faulted;
            }
            else if (task.Status.IsCompletedSuccessfully())
            {
                response.Status = DataResponse.ResponseStatus.Succeeded;
            }
            else
            {
                response.Status = DataResponse.ResponseStatus.TimeOut;
            }
            return response;
        }
        
        public async UniTask<PostUserResourceChangeAccess.Response> PostUserResourceChangeData(UserResource.Type userResourceType, int changeAmount)
        {
            PostUserResourceChangeAccess.Request request = new PostUserResourceChangeAccess.Request()
            {
                UserResourceType = userResourceType,
                ChangeAmount = changeAmount
            };
            PostUserResourceChangeAccess.Response response = new PostUserResourceChangeAccess.Response()
            {
                UserResourceType = userResourceType
            };
            int currentValue = 0;
            int newValue = 0;

            UniTask<DataSnapshot> task = UserResourceReference
                .Child(request.UserResourceType.ToString())
                .RunTransaction(mutableData =>
                {
                    if (mutableData.Value != null)
                    {
                        currentValue = int.Parse(mutableData.Value.ToString());
                    }
                    newValue = currentValue + request.ChangeAmount;
                    mutableData.Value = newValue;
                    return TransactionResult.Success(mutableData);
                })
                .AsUniTask();
            await task.RunWithTimeOut(TimeOutDuration);
            if (task.Status.IsFaulted())
            {
                Debug.Log("IsFaulted");
                response.Status = DataResponse.ResponseStatus.Faulted;
            }
            else if (task.Status.IsCompletedSuccessfully())
            {
                Debug.Log("IsCompletedSuccessfully");
                response.Status = DataResponse.ResponseStatus.Succeeded;
                response.UserResourceType = request.UserResourceType;
                response.CurrentAmount = newValue;
                InGameDataManager.Instance.UserData.UserResourceList[request.UserResourceType].Amount.Value = newValue;
            }
            else
            {
                Debug.Log("TimeOut");
                response.Status = DataResponse.ResponseStatus.TimeOut;
            }
            return response;
        }

        #endregion
    }
    public static class UniTaskExtensions
    {
        public static bool IsFaulted(this UniTaskStatus status)
        {
            return status == UniTaskStatus.Faulted;
        }
        public static bool IsCompletedSuccessfully(this UniTaskStatus status)
        {
            return status == UniTaskStatus.Succeeded;
        }
        public static async UniTask<T> RunWithTimeOut<T>(this UniTask<T> task, int milliseconds)
        {
            await ActivityContainer.Find(ContainerKey.OverlayActivity).ShowAsync(nameof(ActivityWaitTaskRunning));
            (bool hasResultLeft, T result) value = await UniTask.WhenAny(task, DelayTask(milliseconds));
            await ActivityContainer.Find(ContainerKey.OverlayActivity).HideAsync(nameof(ActivityWaitTaskRunning));
            return value.hasResultLeft ? value.result : default;
        }
        public static async UniTask RunWithTimeOut(this UniTask task, int milliseconds)
        {
            await ActivityContainer.Find(ContainerKey.OverlayActivity).ShowAsync(nameof(ActivityWaitTaskRunning));
            await UniTask.WhenAny(task, DelayTask(milliseconds));
            await ActivityContainer.Find(ContainerKey.OverlayActivity).HideAsync(nameof(ActivityWaitTaskRunning));
        }

        private static async UniTask DelayTask(int milliseconds)
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            await UniTask.Delay(milliseconds, cancellationToken: cts.Token);
        }
    }
}