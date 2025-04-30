using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash: StateBase<B1_1_Ctrller>
{
    public float dashDuration;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Teleport(animator);
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

    void Teleport(Animator animator){
        Vector2 targetPos=ctrller.redHat.transform.position;
        //create sequence
        Sequence s=DOTween.Sequence();
        //dash toward redhat
        s.Append(ctrller.transform.DOMove(targetPos, dashDuration));
        s.AppendCallback(()=>{
            ctrller.redHat.gameObject.SetActive(false);
            animator.SetTrigger("toIdle");
            });
    }
}
