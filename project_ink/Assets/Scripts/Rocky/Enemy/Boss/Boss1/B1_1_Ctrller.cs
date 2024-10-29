using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Ctrller : EnemyBase
{
    public GameObject redHat;
    [Header("Action1_2")]
    public Vector2 a1_2_point1;
    public Vector2 a1_2_point2;
    //[Header("Action2")]

    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(a1_2_point1, .5f);
        Gizmos.DrawWireSphere(a1_2_point2, .5f);
    }
    void Start(){
        redHat.SetActive(false);
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.D)){
        }
    }
    public override void OnHit(Projectile proj)
    {
    }
}
