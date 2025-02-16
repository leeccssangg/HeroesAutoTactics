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

public class UIDailyRewardInfo : ACachedMonoBehaviour
{
    [Header("UI Daily Reward Info")]
    [SerializeField] private DailyReward m_Gift;
    [SerializeField] private Image m_ImgIcon;

    [SerializeField] private GameObject m_PanelCollected;
    [SerializeField] private GameObject m_PanelNotReadyCollect;
    [SerializeField] private GameObject m_PanelMissing;
    [SerializeField] private GameObject m_PanelReadyCollect;

    [SerializeField] private TextMeshProUGUI m_TextAmount;
    [SerializeField] private TextMeshProUGUI m_TextDayNum;

    [SerializeField] private Button m_BtnClaim;
    [SerializeField] private Button m_BtnCollectMissing;


    private void Awake()
    {
        m_BtnClaim.SetOnClickDestination(OnClickBtnClaim);
        m_BtnCollectMissing.SetOnClickDestination(OnClickBtnCollectMissing);
    }
    public void Setup(DailyReward gift)
    {
        m_Gift = gift;
        m_TextDayNum.SetText($"{m_Gift.dayID + 1}");
        m_TextAmount.SetText($"{(m_Gift.defaultGift.Amount.Value)}");
        //m_ImgIcon.sprite = m_Gift.defaultGift.ResourceType.GetSprite();
        m_PanelCollected.gameObject.SetActive(DailyGiftManager.Instance.IsCollectedDayID(m_Gift.dayID));
        m_PanelNotReadyCollect.gameObject.SetActive(DailyGiftManager.Instance.IsNotReadyCollectDayId(m_Gift.dayID));
        m_PanelMissing.gameObject.SetActive(DailyGiftManager.Instance.IsMissingDay(m_Gift.dayID));
        m_PanelReadyCollect.gameObject.SetActive(m_Gift.dayID == DailyGiftManager.Instance.GetCurrentDayId() &&
            DailyGiftManager.Instance.IsGoodToClaim(m_Gift.dayID));
        m_BtnClaim.gameObject.SetActive(m_Gift.dayID == DailyGiftManager.Instance.GetCurrentDayId() &&
            DailyGiftManager.Instance.IsGoodToClaim(m_Gift.dayID));
    }
    private async UniTask OnClickBtnClaim()
    {
        m_BtnClaim.interactable = false;
        await ModalDailyGiftContext.Events.ClaimDailyGift.Invoke(m_Gift.dayID);
        m_BtnClaim.interactable = true;
    }
    private async UniTask OnClickBtnCollectMissing()
    {
        m_BtnCollectMissing.interactable = false;
        await ModalDailyGiftContext.Events.ClaimDailyGift.Invoke(m_Gift.dayID);
        m_BtnCollectMissing.interactable = true;
    }
}
