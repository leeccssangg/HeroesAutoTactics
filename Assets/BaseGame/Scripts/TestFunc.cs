using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Manager;

public class TestFunc : MonoBehaviour
{
    [Button]
    public async UniTask TestChangeResource()
    {
        var response = await DatabaseManager.Instance.PostUserResourceChangeData(UserResource.Type.Gem, 10);
        if (response.IsFaulted)
        {
            UnityEngine.Debug.Log("Failed to change resource");
        }
        else if (response.IsSucceeded)
        {
            Debug.Log(InGameDataManager.Instance.UserData.UserResourceList[UserResource.Type.Gem].Amount.Value);
            Debug.Log(response.CurrentAmount);
        }
    }
}