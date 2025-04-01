using System;
using System.Collections.Generic;
using UnityEngine;

public class E_Pig : EliteBase_Ground
{
    [Header("Attack")]
    public float jumpXMin;
    public float jumpXMax;
    public float jumpHeight;
    public float ac1_jumpInterval;
    public float ac2_moveSpeed;
    [Header("Anim")]
    public float animInterval;
    public float ac2_anticipation;
    public float animScaleYMin, animScaleXMax, animStretchY, animStretchX;
    public Collider2D[] pig; //0 is lower
    [NonSerialized][HideInInspector] public Animator[] animators;
    internal override void Start()
    {
        base.Start();
        animators=new Animator[pig.Length];
        for(int i=0;i<pig.Length;++i){
            animators[i]=pig[i].GetComponent<Animator>();
        }
    }
}
