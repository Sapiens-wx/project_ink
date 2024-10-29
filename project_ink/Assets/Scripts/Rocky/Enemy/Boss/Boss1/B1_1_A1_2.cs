using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_A1_2 : StateBase<B1_1_Ctrller>
{
    public float toP1Duration,toP2Duration,backDuration;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action(animator);
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
    void Action(Animator animator){
        Vector3 originalPos=ctrller.transform.position;
        Sequence s=DOTween.Sequence();
        s.Append(ctrller.transform.DOMove(ctrller.a1_2_point1,toP1Duration).SetEase(Ease.InQuad));
        s.Append(ctrller.transform.DOMove(ctrller.a1_2_point2, toP2Duration).SetEase(Ease.Linear));
        s.Append(ctrller.transform.DOMove(originalPos, backDuration));
        s.AppendCallback(()=>{
            animator.SetTrigger("toIdle");
        });
        s.Play();
    }
}
