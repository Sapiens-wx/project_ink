using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_A4_5_S2 : StateBase<B1_1_Ctrller>
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //randomly choose an attack for the redHat
        if(Random.Range(0,2)==0) Action1();
        else Action2();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    void Action1(){
        //shoot 3 bullets toward the player
        ctrller.redHat.animator.SetInteger("to_throw_S2",3);
    }
    void Action2(){
        //shoot 3 bullets toward the player
        ctrller.redHat.animator.SetTrigger("to_throwUp_S2");
    }
}
