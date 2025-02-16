using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public class UserResource
    {
        public enum Type
        {
            Gem = 0,
            Gold = 1,
            MythicPiece = 2,
            GachaPiece = 3,
        }
        [field: SerializeField, HideLabel] public Type ResourceType {get; private set;}
        [field: SerializeField, HideLabel] public ReactiveValue<int> Amount {get; private set;}

        public UserResource(Type resourceType, ReactiveValue<int> amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }
    
    [System.Serializable]
    public class UserResourceList
    {
        [field: SerializeField, HideLabel] public List<UserResource> UserResources {get; private set;}
        
        public UserResource this[UserResource.Type type] => UserResources[(int)type];

        public UserResourceList()
        {
            UserResources = new List<UserResource>();
            UserResource.Type[] types = (UserResource.Type[])System.Enum.GetValues(typeof(UserResource.Type));
            foreach (UserResource.Type type in types)
            {
                UserResources.Add(new UserResource(type, new ReactiveValue<int>(0)));
            }
        }
        public UserResource GetResource(UserResource.Type type)
        {
            return UserResources[(int)type];
        } 
        public void TryAddResource(UserResource.Type type, int amount)
        {
            UserResource userResource = UserResources[(int)type];
            userResource.Amount.Value += amount;
        }

    }
}