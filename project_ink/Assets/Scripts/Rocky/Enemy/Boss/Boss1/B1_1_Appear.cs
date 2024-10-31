using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class B1_1_Appear : StateBase<B1_1_Ctrller>
{
    [SerializeField] float waitTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action1(animator);
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

    void Action1(Animator animator){
        //mark a position
        ctrller.a3_target.transform.position=PlayerShootingController.inst.transform.position;
        ctrller.a3_target.SetActive(true);

        Sequence s = DOTween.Sequence();
        //wait for n sec
        s.AppendInterval(waitTime);
        //Appear at the target and setactive false of target
        s.AppendCallback(()=>{
            ctrller.transform.position=ctrller.a3_target.transform.position;
            ctrller.a3_target.SetActive(false);
            });
        //return to idle state
        s.AppendCallback(()=>animator.SetTrigger("toIdle"));
    }
}
