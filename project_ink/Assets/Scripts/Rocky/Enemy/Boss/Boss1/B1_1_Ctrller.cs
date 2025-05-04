using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class B1_1_Ctrller : BossBase
{
    public B1_1_RedHat redHat;
    [Header("Action1")]
    public float a1_2_jumpHeight;
    [Header("Action3")]
    public GameObject a3_target;
    [Header("Action4")]
    public float a4_redHatShootDist;
    public float a4_redHatShootDuration;
    [Header("Platform")]
    public Bounds platform1;
    public Bounds platform2;

    [NonSerialized][HideInInspector] public bool a4_s2_redhatFinishThrow;
    protected override void OnDrawGizmosSelected(){
        base.OnDrawGizmosSelected();
        Gizmos.DrawWireCube(platform1.center,platform1.size);
        Gizmos.DrawWireCube(platform2.center,platform2.size);
        Gizmos.DrawLine(new Vector2(-30,a1_2_jumpHeight), new Vector2(30,a1_2_jumpHeight));
    }
    internal override void Start(){
        base.Start();
        redHat.boss=this;
        redHat.gameObject.SetActive(false);
        a3_target.SetActive(false);
    }
    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);
        if(CurHealth<<1<maxHealth) //enter stage 2
            animator.SetTrigger("toIdle2");
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
