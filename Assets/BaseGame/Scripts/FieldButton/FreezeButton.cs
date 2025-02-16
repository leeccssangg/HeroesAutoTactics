using Game.Core;
using Game.Manager;
using UnityEngine.EventSystems;
using Zenject;

public class FreezeButton : FieldButton
{
    [Inject] private InputManager InputManager { get; set; }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        InputManager.SetCurrentHolderOverride(this);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (InputManager.CurrentHolderOverride != this) return;
        InputManager.SetCurrentHolderOverride(null);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (!IsInteractable) return;
        if (InputManager.SelectedSlot is not StoreSlot shopSlot) return;
        if (!shopSlot.HasHero) return;
        shopSlot.ToggleFreeze();
    }
}