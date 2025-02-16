using System;
using UnityEngine;

[Serializable]
public class StatusEffectStack
{
    [field: SerializeField] public Status Status { get; set; }

    [field: SerializeField] public int Stacks { get; set; }

    public StatusEffectStack()
    {
    }

    public StatusEffectStack(Status status, int stack)
    {
        Status = status;
        Stacks = stack;
    }

}