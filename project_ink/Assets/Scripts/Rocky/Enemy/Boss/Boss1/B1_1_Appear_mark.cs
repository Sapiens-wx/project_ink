using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class B1_1_Appear_mark : StateBase<B1_1_Ctrller>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //mark a position
        ctrller.a3_target.transform.position=PlayerShootingController.inst.transform.position;
        ctrller.a3_target.SetActive(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            ctrller.a3_target.SetActive(false);
    }
}
