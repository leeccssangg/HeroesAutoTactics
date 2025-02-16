using Game.Manager;
using UnityEngine;using UnityEngine.EventSystems;
using Zenject;

public class UnSelectButton : MonoBehaviour, IPointerClickHandler
{
    [Inject] private InputManager InputManager { get; set; }
    public void OnPointerClick(PointerEventData eventData)
    {
        InputManager.InActiveSelect();
    }
}