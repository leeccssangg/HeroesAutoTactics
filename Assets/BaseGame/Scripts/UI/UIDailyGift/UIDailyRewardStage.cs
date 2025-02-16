using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using TW.Utility.CustomComponent;
using Game.Manager;
using Game.Data;
using TW.Reactive.CustomComponent;
using R3;
using Cysharp.Threading.Tasks;

public class UIDailyRewardStage : ACachedMonoBehaviour
{
    [Header("UI Daily Reward Info")]
    [SerializeField] private DailyGiftStage m_Gift;
    [SerializeField] private Image m_ImgIcon;

    [SerializeField] private GameObject m_PanelCollected;
    [SerializeField] private GameObject m_PanelNotReadyCollect;
    [SerializeField] private GameObject m_PanelReadyCollect;

    [SerializeField] private TextMeshProUGUI m_TextAmount;
    [SerializeField] private TextMeshProUGUI m_TextDayNum;

    [SerializeField] private Button m_BtnClaim;

    private void Awake()
    {
        m_BtnClaim.SetOnClickDestination(OnClickBtnClaim);
    }
    public void Setup(DailyGiftStage gift)
    {
        m_Gift = gift;
        m_TextDayNum.SetText($"{m_Gift.dayCount}");
        m_TextAmount.SetText($"{(m_Gift.reward.Amount.Value)}");
        //m_ImgIcon.sprite = m_Gift.defaultGift.ResourceType.GetSprite();
        int curStage = DailyGiftManager.Instance.Stage;
        bool isClaimable = (DailyGiftManager.Instance.GetListDailyGiftStages().IndexOf(m_Gift) == curStage) &&
                       DailyGiftManager.Instance.IsGoodToClaimStageReward();
        bool isClaimed = (DailyGiftManager.Instance.Stage == DailyGiftGlobalConfig.Instance.stages.Count)
            || gift.dayCount < DailyGiftManager.Instance.GetListDailyGiftStages()[curStage].dayCount;
        m_PanelReadyCollect.SetActive(isClaimable);
        m_PanelCollected.SetActive(isClaimed);
        m_PanelNotReadyCollect.SetActive(!isClaimable && !isClaimed);
        m_BtnClaim.gameObject.SetActive(isClaimable);
    }
    private async UniTask OnClickBtnClaim()
    {
        m_BtnClaim.interactable = false;
        await ModalDailyGiftContext.Events.ClaimDailyGiftStage.Invoke();
        m_BtnClaim.interactable = true;

    }
}
