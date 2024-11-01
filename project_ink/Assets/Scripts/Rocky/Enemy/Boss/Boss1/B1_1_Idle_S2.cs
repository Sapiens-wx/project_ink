using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_1_Idle_S2 : StateBase<B1_1_Ctrller>
{
    [Header("Debug usage: enable action")]
    public bool a1;
    public bool a2, a3, a4, a5;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch(Random.Range(0,5)){
            case 0:
                if(a1) animator.SetTrigger("toA1");
                break;
            case 1:
                if(a2) animator.SetTrigger("toA2");
                break;
            case 2:
                if(a3) animator.SetTrigger("toA3");
                break;
            case 3:
                if(a4) animator.SetTrigger("toA4");
                break;
            case 4:
                if(a5) animator.SetTrigger("toA5");
                break;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
