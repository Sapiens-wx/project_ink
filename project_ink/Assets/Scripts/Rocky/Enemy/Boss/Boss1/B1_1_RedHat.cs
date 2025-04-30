using System;
using UnityEngine;

public class B1_1_RedHat : BossBase
{
    [NonSerialized][HideInInspector] public Animator redHatAnimator;
    [NonSerialized][HideInInspector] public Collider2D redHatbc;
    [NonSerialized][HideInInspector] public B1_1_Ctrller boss;
    internal override void Start(){
        base.Start();
        redHatAnimator=GetComponent<Animator>();
        redHatbc=GetComponent<Collider2D>();
    }
    public override void OnDamaged(int damage)
    {
        boss.OnDamaged(damage); //redirect damage to the boss (the wolf)
    }
    /// <summary>
    /// returns the position [yoffset] above the middle of the platform
    /// </summary>
    public Vector2 OffsetPlatformPos(Bounds platform, float yoffset){
        Vector2 ret=platform.center;
        ret.y+=platform.extents.y+yoffset;
        return ret;
    }
}