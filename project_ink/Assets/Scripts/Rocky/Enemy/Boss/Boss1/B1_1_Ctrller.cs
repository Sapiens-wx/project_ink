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
    [SerializeField] Vector2 a4_redHatShootPos;
    public float a4_redHatShootDuration;
    [SerializeField] Vector2 stomachPos;
    [Header("Platform")]
    public Bounds platform1;
    public Bounds platform2;

    [NonSerialized][HideInInspector] public bool a4_s2_redhatFinishThrow;
    public Vector2 StomachGlobalPos{
        get{
            Vector2 pos=stomachPos + (Vector2)transform.position;
            if(Dir==1) pos=new Vector2(transform.position.x-stomachPos.x,stomachPos.y+transform.position.y);
            else pos=stomachPos + (Vector2)transform.position;
            return pos;
        }
    }
    public Vector2 A4_redHatShootGlobalPos => a4_redHatShootPos + (Vector2)transform.position;
    protected override void OnDrawGizmosSelected(){
        base.OnDrawGizmosSelected();
        Gizmos.DrawWireCube(platform1.center,platform1.size);
        Gizmos.DrawWireCube(platform2.center,platform2.size);
        Gizmos.DrawLine(new Vector2(-30,a1_2_jumpHeight), new Vector2(30,a1_2_jumpHeight));
        Gizmos.color=Color.blue;
        Gizmos.DrawWireSphere(StomachGlobalPos, .3f);
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(A4_redHatShootGlobalPos, .3f);
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
