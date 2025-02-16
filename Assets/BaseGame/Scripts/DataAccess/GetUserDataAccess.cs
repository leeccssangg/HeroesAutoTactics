using System;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Game.Data;
using Game.Manager;
using UnityEngine;

namespace Game.DataAccess
{
    public static class GetUserDataAccess
    {
        public class Request : DataRequest
        {
            public string UserId { get; private set; }

            public Request(string userId)
            {
                UserId = userId;
            }
        }

        public class Response : DataResponse
        {
            public UserData UserData { get; set; }
        }
    }
}