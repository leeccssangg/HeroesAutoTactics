using DG.Tweening;
using Game.GlobalConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

public class UIHeroGacha : ACachedMonoBehaviour
{
    [field: SerializeField] public SummonHeroReward SummonHeroReward { get; private set; }
    [field: SerializeField] public Image ImgIconHero { get; private set; }
    [field: SerializeField] public Transform StartPos { get; private set; }
    [field: SerializeField] public Transform EndPos { get; private set; }
    [field: SerializeField] public Action ActionLastSummoned { get; private set; } = null;

    public void Setup(SummonHeroReward summonHeroReward,Transform startPos, Transform endPos)
    {
        SummonHeroReward = summonHeroReward;
        StartPos = startPos;
        EndPos = endPos;
        Transform.position = StartPos.position;
        SetupIconHero();
        //ImgIconHero.sprite = Resources.Load<Sprite>($"HeroIcon/{summonHeroReward.Config.Type}");
    }
    //public void SetupActionLastSummoned(Action action)
    //{
    //    ActionLastSummoned = action;
    //}
    public void InitDelay(float timeWait)
    {
        DOVirtual.DelayedCall(timeWait, StartMoving);
    }
    private void StartMoving()
    {
        this.Transform.DOMoveX(EndPos.position.x, Vector3.Distance(StartPos.position,EndPos.position)/5).From(StartPos.position).SetEase(Ease.Linear)
            .OnComplete
            (
            () => 
            {
                if (SummonHeroReward.IsAcquired)
                {
                    ModalGachaHeroContext.Events.SpawnUIGachaReward?.Invoke(SummonHeroReward);
                }
                //ActionLastSummoned?.Invoke();
                ModalGachaHeroContext.Events.CheckCompletedAnim?.Invoke();
                this.gameObject.SetActive(false);
            }
            );
    }
    private void SetupIconHero()
    {
        if(SummonHeroReward.Synergy != Combat.Family.None)
        {
            ImgIconHero.sprite = SummonHeroReward.RewardHero.SpriteIcon;
        }
    }
}
