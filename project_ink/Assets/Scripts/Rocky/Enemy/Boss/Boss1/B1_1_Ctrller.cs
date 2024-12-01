using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Ctrller : BossBase
{
    public GameObject redHat;
    [Header("Action1")]
    public float a1_2_jumpHeight;
    [Header("Action3")]
    public GameObject a3_target;
    [Header("Action4")]
    public GameObject a4_bullet;
    [Header("Action5")]
    public GameObject a5_bullet;
    [Header("Stage Transit")]
    public Vector2 st_platform1;
    public Vector2 st_platform2;

    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(st_platform1, .5f);
        Gizmos.DrawWireSphere(st_platform2, .5f);
        Gizmos.DrawLine(new Vector2(-30,a1_2_jumpHeight), new Vector2(30,a1_2_jumpHeight));
    }
    internal override void Start(){
        base.Start();
        redHat.SetActive(false);
        a3_target.SetActive(false);
    }
    public override void OnHit(Projectile proj)
    {
        base.OnHit(proj);
        if(CurHealth<<1<maxHealth)
            animator.SetTrigger("toIdle2");
    }
}
