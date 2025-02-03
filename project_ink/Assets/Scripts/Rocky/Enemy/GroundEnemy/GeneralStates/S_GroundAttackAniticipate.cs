using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1. keep facing at the player
public class S_GroundAttackAnticipate : StateBase<EnemyBase_Ground>
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Mathf.Sign(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x)!=ctrller.Dir){
            ctrller.Dir=-ctrller.Dir;
        }
    }
}