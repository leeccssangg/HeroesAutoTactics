using Game.Core;
using Game.Manager;
using UnityEngine.EventSystems;
using Zenject;

public class RollButton : FieldButton
{
    [Inject] InputManager InputManager { get; set; }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (!IsInteractable) return;
        if (InputManager.SelectedSlot is StoreSlot)
        {
            InputManager.InActiveSelect();
        }

    }
}