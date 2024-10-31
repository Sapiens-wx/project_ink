using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Ctrller : EnemyBase
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
    //[Header("Action2")]

    void OnDrawGizmosSelected(){
        Gizmos.DrawLine(new Vector2(-30,a1_2_jumpHeight), new Vector2(30,a1_2_jumpHeight));
    }
    void Start(){
        redHat.SetActive(false);
        a3_target.SetActive(false);
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.D)){
        }
    }
    public override void OnHit(Projectile proj)
    {
    }
}
