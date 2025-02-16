using LitMotion;
using TMPro;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using Zenject;

public class CombatText : ACachedMonoBehaviour
{
    [field: SerializeField] public TextMeshPro[] Text {get; private set;}
    [field: SerializeField] public SpriteRenderer Background {get; private set;}
    [field: SerializeField] private Vector3 EndPositionOffset {get; set;}
    [field: SerializeField] private float Noise {get; set;}
    [field: SerializeField] public FloatRange Duration {get; private set;}
    [field: SerializeField] private AnimationCurve MoveCurve {get; set;}
    [field: SerializeField] private AnimationCurve ScaleCurve {get; set;}
    [field: SerializeField] private AnimationCurve AlphaCurve {get; set;}
    public class Factory : PlaceholderFactory<Object, CombatText>
    {
        public static Factory CreateInstance()
        {
            return new Factory();
        }
    }
    public CombatText SetText(params string[] text)
    {
        for (int i = 0; i < Text.Length; i++)
        {
            Text[i].text = text[i];
        }
        return this;
    }
    public void PlayAnimation(Vector3 startPosition, TickRate tickRate)
    {
        Transform.localScale = Vector3.zero;
        Transform.position = startPosition;
        
        Vector3 endPosition = startPosition + EndPositionOffset + (Vector3)Random.insideUnitCircle * Noise;
        LMotion.Create(0f, 1f, Duration.GetRandomValue() / tickRate.ToValue())
            .WithEase(Ease.Linear)
            .WithOnComplete(SelfDespawn)
            .Bind(UpdateAnimation);
        return;
        void UpdateAnimation(float value)
        {
            Transform.localScale = Vector3.one * ScaleCurve.Evaluate(value);
            Transform.position = Vector3.LerpUnclamped(startPosition, endPosition, MoveCurve.Evaluate(value));
            float alpha = AlphaCurve.Evaluate(value);
            for (int i = 0; i < Text.Length; i++)
            {
                Text[i].alpha = alpha;
            }
            Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, alpha);
        }
    }
    private void SelfDespawn()
    {
        Destroy(gameObject);
    }
}