using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_1_A5_Bullet : EnemyBulletBase
{
    protected override void OnTriggerEnter2D(Collider2D collider){
        InvokeOnTriggerEnterEvent(collider);
        //if is going upward and touches the ground, do not destroy
        if(!(GameManager.IsLayer(GameManager.inst.groundLayer, collider.gameObject.layer) && rgb.velocity.y>0))
            StartCoroutine(DelayDestroy());
    }
}
