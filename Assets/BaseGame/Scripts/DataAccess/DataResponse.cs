using Firebase.Database;
using UnityEngine;

namespace Game.DataAccess
{
    public abstract class DataResponse
    {
        public enum ResponseStatus
        {
            Faulted,
            Succeeded,
            TimeOut,
        }
        public DataRequest DataRequest {get; private set;}
        public ResponseStatus Status {get; set;}
        public bool IsFaulted => Status is ResponseStatus.Faulted or ResponseStatus.TimeOut;
        public bool IsSucceeded => Status == ResponseStatus.Succeeded;
    }
}