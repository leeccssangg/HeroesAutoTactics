using Game.GlobalConfig;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGachaReward : MonoBehaviour
{
    [field: SerializeField] public SummonHeroReward Reward { get; private set; }
    [field: SerializeField] public Image ImgReward { get; private set; }
    [field: SerializeField] public Image ImgRewardBg { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtRewardAmout { get; private set; }
    [field: SerializeField] public FeelAnimation AnimOff { get; private set; }
    [field: SerializeField] public FeelAnimation AnimOn { get; private set; }

    public void Setup(SummonHeroReward reward)
    {
        Reward = reward;
        TxtRewardAmout.SetText($"{Reward.Config.RewardAmount * Reward.NumGacha}");
        SetupIconHero();
    }
    private void SetupIconHero()
    {
        if (Reward.Synergy != Combat.Family.None)
        {
            ImgReward.sprite = Reward.RewardHero.SpriteIcon;
        }
    }
    public void PlayAnim(bool isOn)
    {
        if (isOn)
        {
            AnimOn.Play();
        }
        else
        {
            AnimOff.Play();
        }
    }
}
