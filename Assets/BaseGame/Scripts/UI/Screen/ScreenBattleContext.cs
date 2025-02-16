using System;
using Cysharp.Threading.Tasks;
using Game.Data;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;

[Serializable]
public class ScreenBattleContext 
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
        [field: SerializeField] public BattleData PlayerBattleData { get; private set; }
        [field: SerializeField] public BattleData OpponentBattleData { get; private set; }
        
        public UniTask Initialize(Memory<object> args)
        {   
            PlayerBattleData = (BattleData)args.Span[0];
            OpponentBattleData = (BattleData)args.Span[0];
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextOpponentName {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextOpponentTeamName {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextOpponentHeart {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextOpponentTurn {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextOpponentWin {get; private set;}
        
        [field: SerializeField] public TextMeshProUGUI TextPlayerName {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerTeamName {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerHeart {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerTurn {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerWin {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {

            
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            
            View.TextOpponentHeart.text = Model.OpponentBattleData.Heart.ToString();
            View.TextOpponentTurn.text = Model.OpponentBattleData.Round.ToString();
            View.TextOpponentWin.text =$"{Model.OpponentBattleData.Win.ToString()}/10";
            
            View.TextPlayerHeart.text = Model.PlayerBattleData.Heart.ToString();
            View.TextPlayerTurn.text = Model.PlayerBattleData.Round.ToString();
            View.TextPlayerWin.text =$"{Model.PlayerBattleData.Win.ToString()}/10";
            
        }      
    }
}