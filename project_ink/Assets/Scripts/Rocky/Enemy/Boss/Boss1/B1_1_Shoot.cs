using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Shoot : StateBase<B1_1_Ctrller>
{
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
        ctrller.redHat.transform.position=ctrller.StomachGlobalPos;
        ctrller.redHat.gameObject.SetActive(true);
        Sequence s = DOTween.Sequence();
        //shoot redhat upward
        s.Append(ctrller.redHat.transform.DOMove(ctrller.A4_redHatShootGlobalPos, ctrller.a4_redHatShootDuration));
        //scale the redHat from 0 to 1
        Vector3 toScale=Vector3.one;
        if(PlayerCtrl.inst.transform.position.x>ctrller.transform.position.x)
            toScale.x=-1;
        ctrller.redHat.transform.localScale=Vector3.zero;
        s.Join(DOTween.To(()=>{return Vector3.zero;},
            val=>{ctrller.redHat.transform.localScale=val;},
            toScale, ctrller.a4_redHatShootDuration));
        //s.Join(ctrller.redHat.transform.DOScale(Vector3.one, redHatShootDuration));
        //shoot 3 bullets toward the player
        ctrller.redHat.animator.SetInteger("to_throw",3);
    }
}
