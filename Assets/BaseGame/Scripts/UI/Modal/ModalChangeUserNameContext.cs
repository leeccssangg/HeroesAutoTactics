using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using Pextension;
using TW.UGUI.Core.Sheets;
using UnityEditor.SceneManagement;
//using UGUI.Core.Modals;
using Game.Data;
using Game.Manager;
using Game.GlobalConfig;
using DG.Tweening;
using Combat;
using TW.UGUI.Core.Activities;

[Serializable]
public class ModalChangeUserNameContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public Button BtnChangeName { get; private set; }
        [field: SerializeField] public TMP_InputField InputField { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtCurrentName { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            BtnChangeName.interactable = false;
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            View.TxtCurrentName.SetText($"{PlayerProfileManager.Instance.UserName.Value}");
            View.InputField.onValueChanged.AddListener((_) => OnValueInputFieldChange());

            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
            View.BtnChangeName.SetOnClickDestination(OnClickBtnChangeName);
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
        private void OnClickBtnChangeName(Unit _)
        {
            //UserData userData = GameManager.Instance.UserData;
            //userData.UserName = View.InputField.text;
            //GameManager.Instance.UserData = userData;
            if(IsUserNameValid(View.InputField.text))
            {
                //GameManager.Instance.SaveUserDataAsync(userData);
                PlayerProfileManager.Instance.ChangeUserName(View.InputField.text);
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            }
            else
            {
                //Show error message
                return;
            }
        }
        private bool IsUserNameValid(string userName)
        {
            if (userName.Length < 2 || userName.Length > 10)
            {
                return false;
            }
            if(userName.Contains(" "))
            {
                return false;
            }
            if(userName.IsNullOrWhitespace())
            {
                return false;
            }
            if(!PlayerProfileManager.Instance.IsAlphanumeric(userName))
            {
                return false;
            }
            return true;
        }
        private bool IsEnoughCost()
        {
            //TODO check cost resource
            return true;
        }
        public void OnValueInputFieldChange()
        {
            View.BtnChangeName.interactable = IsUserNameValid(View.InputField.text) && IsEnoughCost();
        }
    }
}