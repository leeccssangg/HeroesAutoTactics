using System;
using Game.Core;
using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldButton : Holder
{
    [field: Title(nameof(FieldButton))]
    [field: SerializeField] public bool IsInteractable {get; private set;}
    [field: SerializeField] private Transform Graphic {get; set;}
    [field: SerializeField] public GameObject OverrideObject {get; private set;}
    private float CurrentScale { get; set; } = 1f;
    private MotionHandle ScaleMotionHandle { get; set; }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
        OverrideObject.SetActive(false);
    }
    public void SetInteractable(bool isInteractable)
    {
        IsInteractable = isInteractable;
    }
    private void SetCurrentScale(float scale)
    {
        CurrentScale = scale;
        Graphic.localScale = Vector3.one * scale;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {

    }

    public override void OnPointerUp(PointerEventData eventData)
    {

    }

    public override void OnPointerClick(PointerEventData eventData)
    {

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        OverrideObject.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        OverrideObject.SetActive(false);
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
}