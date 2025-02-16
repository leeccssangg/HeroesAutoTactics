using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeartPlayer : MonoBehaviour
{
    [field: SerializeField] public FeelAnimation AnimInitLose { get; private set; }
    [field: SerializeField] public FeelAnimation AnimLoseHeart { get; private set; }

    public void PlayAnimInitLose()
    {
        AnimInitLose.Play();
    }
    public void PlayAnimLoseHeart()
    {
        AnimLoseHeart.Play();
    }
}
