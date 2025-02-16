using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.Utility.DesignPattern;
using TW.Reactive.CustomComponent;
using Game.Data;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;

namespace Game.Manager
{
    public class PlayerProfileManager : Singleton<PlayerProfileManager>
    {
        [field: SerializeField] public ReactiveValue<string> UserName { get; private set; }
        [field: SerializeField] public ReactiveValue<Rank> Rank { get; private set; }
        [field: SerializeField] public ReactiveValue<int> RankPoint { get; private set; }

        private async void Start()
        {
            await UniTask.WaitUntil(InGameDataManager.Instance.IsLoadingCompleteCache);
            LoadData();
        }
        private void LoadData()
        {
            UserName = new(InGameDataManager.Instance.UserData.UserName);
            Rank = new(InGameDataManager.Instance.UserData.Rank);
            RankPoint = new(InGameDataManager.Instance.UserData.RankPoint);
        }
        public void ChangeUserName(string userName)
        {
            UserName.Value = userName;
        }
        public bool IsAlphanumeric(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Regular expression to match only letters and numbers
            string pattern = @"^[a-zA-Z0-9]+$";
            return Regex.IsMatch(input, pattern);
        }
    }
}

