using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Game.Data;
using UnityEngine;

namespace Game.DataAccess
{
    public static class PostPlayerBattleDataAccess
    {
        public class Request : DataRequest
        {
            public BattleData BattleData { get; private set; }

            public Request(BattleData battleData)
            {
                BattleData = battleData;
            }
        }

        public class Response : DataResponse
        {

        }
    }
}