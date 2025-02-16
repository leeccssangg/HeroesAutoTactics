using System;
using UnityEngine;

[Serializable]
public class AttackTemp
{
    [field: SerializeField] public Targeting TargetType { get; set; }

    [field: SerializeField] public AttackEffect AttackEffect { get; set; }

    [field: SerializeField] public int Value1 { get; set; }
    [field: SerializeField] public int Value2 { get; set; }
    [field: SerializeField] public int Value3 { get; set; }

    [field: SerializeField] public Anim Anim { get; set; }
    [field: SerializeField] public Sprite Projectile { get; set; }
    

    public AttackTemp(Targeting targeting, AttackEffect attackEffect, int value, Anim anim)
    {
        TargetType = targeting;
        AttackEffect = attackEffect;
        Value1 = value;
        Anim = anim;
    }
    public AttackTemp(AttackTemp attackTemp)
    {
        TargetType = attackTemp.TargetType;
        AttackEffect = attackTemp.AttackEffect;
        Value1 = attackTemp.Value1;
        Value2 = attackTemp.Value2;
        Value3 = attackTemp.Value3;
        Anim = attackTemp.Anim;
        Projectile = attackTemp.Projectile;
    }

}
