using UnityEngine;

public class SpriteSlider : MonoBehaviour
{
    [field: SerializeField] public Transform Handler {get; private set;}
    [field: SerializeField] public Vector3 StartPosition {get; private set;}
    [field: SerializeField] public Vector3 EndPosition {get; private set;}
    [field: SerializeField] public float Value {get; private set;}
    
    public void SetValue(float value)
    {
        Value = value;
        Handler.localPosition = Vector3.Lerp(StartPosition, EndPosition, value);
    }
}