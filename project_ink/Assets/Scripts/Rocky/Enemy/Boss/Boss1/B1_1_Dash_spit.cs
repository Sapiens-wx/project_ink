using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_spit : StateBase<B1_1_Ctrller>
{
    public float redHatDashDuration;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        SpitRedHat(animator);
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

    void SpitRedHat(Animator animator){
        //reset redHat's position
        ctrller.redHat.transform.position=ctrller.transform.position;
        ctrller.redHat.SetActive(true);
        ctrller.redHatAnimator.SetTrigger("to_fly");
        //calculate direction
        Vector2 dir=PlayerShootingController.inst.transform.position-ctrller.redHat.transform.position;
        dir.Normalize();
        Vector2 targetPos=(Vector2)PlayerShootingController.inst.transform.position+dir*.3f;
        //create sequence
        Sequence s=DOTween.Sequence();
        //move the redhat toward the player
        s.Append(ctrller.redHat.transform.DOMove(targetPos, redHatDashDuration));
        s.AppendCallback(()=>{
            animator.SetTrigger("to_A2_dash");
            ctrller.redHatAnimator.SetTrigger("to_idle");
            });
    }
}
