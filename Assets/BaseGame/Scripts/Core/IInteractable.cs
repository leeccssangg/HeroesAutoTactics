using UnityEngine.EventSystems;

public interface IInteractable 
{
    void OnPointerDown(PointerEventData eventData);
    void OnPointerUp(PointerEventData eventData);
    void OnPointerClick(PointerEventData eventData);
    void OnPointerEnter(PointerEventData eventData);
    void OnPointerExit(PointerEventData eventData);
    void OnBeginDrag(PointerEventData eventData);
    void OnDrag(PointerEventData eventData);
    void OnEndDrag(PointerEventData eventData);
}