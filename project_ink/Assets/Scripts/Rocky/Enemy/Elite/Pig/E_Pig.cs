using System.Collections.Generic;
using UnityEngine;

public class E_Pig : EliteBase_Ground
{
    [Header("Attack")]
    public float jumpXMin;
    public float jumpXMax;
    public float jumpHeight;
    public float jumpInterval;
    public float ac2_jumpInterval;
    [Header("Anim")]
    public float animInterval;
    public float animScaleYMin, animScaleXMax;
    public Collider2D pig1, pig2, pig3; //1 is lower
    internal override void Start()
    {
        base.Start();
    }
}
