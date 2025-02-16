using Game.Manager;
using UnityEngine.EventSystems;
using Zenject;

public class EndTurnButton : FieldButton
{
    [Inject] GameManager GameManager { get; set; }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (!IsInteractable) return;
        GameManager.EndTurn();
    }
}