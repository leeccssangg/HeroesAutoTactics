using Game.Core;
using Game.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class SellGroup : Holder
{
    private InputManager InputManager => InputManager.Instance;
    [field: Title(nameof(SellGroup))]
    [field: SerializeField] public GameObject HoverObject {get; private set;}
    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
        HoverObject.SetActive(false);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        HoverObject.SetActive(true);
        InputManager.SetCurrentHolderOverride(this);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        HoverObject.SetActive(false);
        if (InputManager.CurrentHolderOverride != this) return;
        InputManager.SetCurrentHolderOverride(null);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public override void OnDrag(PointerEventData eventData)
    {
        
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // if (InputManager.SelectedSlot is not FieldSlot{HasHero: true} fieldSlot) return;
        // fieldSlot.OwnerHero.SelfSellImmediate();
        // fieldSlot.RemoveHero();
        //
        // InputManager.InActiveSelect();
    }
    
}