using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Game.Manager;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class UserData
    {
        [field: SerializeField] public string UserId { get; set; }
        [field: SerializeField] public string UserName { get; set; }
        [field: SerializeField] public Rank Rank { get; set; }
        [field: SerializeField] public int RankPoint { get; set; }
        [field: SerializeField] public UserResourceList UserResourceList { get; private set; }
        [field: SerializeField] public HeroUpgradeData HeroUpgradeData {get; private set;}
        public UserData(string userId)
        {
            UserId = userId;
            UserName = "NewUser";
            Rank = Rank.Iron;
            RankPoint = 0;
            UserResourceList = new UserResourceList();
        }
        public UserData(string userId, string userName, Rank rank, int rankPoint)
        {
            UserId = userId;
            UserName = userName;
            Rank = rank;
            RankPoint = rankPoint;
            UserResourceList = new UserResourceList();
        }
    }
}