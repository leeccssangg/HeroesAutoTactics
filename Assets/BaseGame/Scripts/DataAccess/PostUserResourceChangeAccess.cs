using Game.Data;
using UnityEngine;

namespace Game.DataAccess
{
    public class PostUserResourceChangeAccess
    {
        public class Request : DataRequest
        {
            public UserResource.Type UserResourceType {get; set;}
            public int ChangeAmount {get; set;}
        }

        public class Response : DataResponse
        {
            public UserResource.Type UserResourceType {get; set;}
            public int CurrentAmount {get; set;}
        }
    }
}