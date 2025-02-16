using System;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.DataAccess;
using TW.Utility.DesignPattern;
using UnityEngine;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using Zenject;

namespace Game.Manager
{
    public partial class InGameDataManager : Singleton<InGameDataManager>
    {
        [Inject] DatabaseManager DatabaseManager { get; set; }
        
        private string m_UserId;
        public string UserId => m_UserId ??= GetUserId();

        
        [field: SerializeField] public UserData UserData { get; private set; }
        [field: SerializeField] public InGameData InGameData { get; private set; }
        private bool IsDataLoadSuccess { get; set; }
        private string GetUserId()
        {
            string userId = ObscuredPrefs.Get(DataKey.UserIdKey, SystemInfo.deviceUniqueIdentifier);
            ObscuredPrefs.Set(DataKey.UserIdKey, userId);
            return userId;
        } 
        [ACacheMethod]
        private bool IsLoadingComplete()
        {
            return IsDataLoadSuccess;
        }
        private void Start()
        {
            LoadData().Forget();
        }

        private async UniTask LoadData()
        {
            await UniTask.Delay(1000);
            IsDataLoadSuccess = false;
            GetUserDataAccess.Response dataResponse = await DatabaseManager.GetUserData(UserId);

            if (dataResponse.IsSucceeded)
            {
                Debug.Log("Data Load Succeeded");
                UserData = dataResponse.UserData;
                IsDataLoadSuccess = true;
            }
            
            if(dataResponse.IsFaulted)
            {
                Debug.Log("Data Load Failed");
                IsDataLoadSuccess = true;
            }
        }
    }

}