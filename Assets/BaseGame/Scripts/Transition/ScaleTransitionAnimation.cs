﻿using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TW.UGUI.Shared
{
    public class ScaleTransitionAnimation : IUnitTransitionAnimation
    {
        public bool IsInitialized { get; set; }
        [field: SerializeField] private RectTransform Owner { get; set; }
        [field: HorizontalGroup("BeforeValue"), HideLabel]
        [field: SerializeField] public Vector3 BeforeValue { get; set; } = Vector3.one;
        [field: HorizontalGroup("AfterValue"), HideLabel]
        [field: SerializeField] public Vector3 AfterValue { get; set; } = Vector3.one;
        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField] public float Delay { get; set; }
        [field: HorizontalGroup("Time"), LabelWidth(80)]
        [field: SerializeField] public float Duration { get; set; } = 0.3f;
        [field: HorizontalGroup("Interpolate"), HideLabel]
        [field: SerializeField] public InterpolateTransition Interpolate { get; set; } = InterpolateTransition.Ease;
        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.Ease")]
        [field: SerializeField, HideLabel] public Ease EaseType { get; set; } = Ease.Linear;
        [field: HorizontalGroup("Interpolate"), ShowIf("@Interpolate == InterpolateTransition.AnimationCurve")]
        [field: SerializeField, HideLabel] public AnimationCurve AnimationCurve { get; set; } = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }

        public void Setup()
        {

        }

        public void SetTime(float time)
        {
            time = Mathf.Max(0.0f, time - Delay);
            float progress = Duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / Duration);
            Vector3 currentValue = Vector3.zero;
            switch (Interpolate)
            {
                case InterpolateTransition.Ease:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, EaseType);
                    break;
                case InterpolateTransition.AnimationCurve:
                    currentValue = DOVirtual.EasedValue(BeforeValue, AfterValue, progress, AnimationCurve);
                    break;
            }
            Owner.localScale = currentValue;
        }
    }
}