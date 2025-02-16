using TW.UGUI.Core.Modals;
using UnityEngine;

public class ModalDailyGift : Modal
{
    [field: SerializeField] public ModalDailyGiftContext.UIPresenter UIPresenter { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();

        AddLifecycleEvent(UIPresenter, 1);
    }
}
