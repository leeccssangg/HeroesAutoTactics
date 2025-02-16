using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICupPlayer : MonoBehaviour
{
    
    [field: SerializeField] public FeelAnimation AnimInitLose { get; private set; }
    [field: SerializeField] public FeelAnimation AnimWinCup { get; private set; }
    [field: SerializeField] public FeelAnimation AnimMoveToChest { get; private set; }

    public void PlayAnimInitLose()
    {
        AnimInitLose.Play();
    }
    public void PlayAnimWinCup()
    {
        AnimWinCup.Play();
    }
    public void PlayAnimMoveToChest()
    {
       AnimMoveToChest.Play();
    }
}
