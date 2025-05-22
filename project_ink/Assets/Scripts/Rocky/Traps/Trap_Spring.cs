using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Spring : TrapBase
{
    [SerializeField] float height;
    void OnTriggerEnter2D(Collider2D collider){
        //if is player or enemy
        if(GameManager.IsLayer(GameManager.inst.playerLayer, collider.gameObject.layer)){
            PlayerCtrl.inst.v_trap+=new Vector2(0, Mathf.Sqrt(19.6f*height));
            PlayerCtrl.inst.animator.SetTrigger("jump_up");
        }
        if(GameManager.IsLayer(GameManager.inst.enemyLayer, collider.gameObject.layer)&&
            collider.GetComponent<EnemyBase_Air>()){
            collider.attachedRigidbody.AddForce(new Vector2(0, Mathf.Sqrt(19.6f*height)));
        }
    }
}
