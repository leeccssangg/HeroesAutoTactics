using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Game.Data;
using TW.Utility.Extension;
using UnityEngine;

namespace Game.DataAccess
{
    public class GetOpponentBattleDataAccess
    {
        public class Request : DataRequest
        {
            public Rank Rank { get; private set; }
            public int Win { get; private set; }
            public int Heart { get; private set; }
            public int Round { get; private set; }

            public Request(Rank rank, int win, int heart, int round)
            {
                Rank = rank;
                Win = win;
                Heart = heart;
                Round = round;
            }
        }

        public class Response : DataResponse
        {
            public BattleData BattleData { get; set; }
        }
    }
}