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
        ctrller.redHat.transform.position=ctrller.transform.position;
        ctrller.redHat.gameObject.SetActive(true);
        Sequence s = DOTween.Sequence();
        //shoot redhat upward
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.redHat.transform.position.y+ctrller.a4_redHatShootDist, ctrller.a4_redHatShootDuration));
        //scale the redHat from 0 to 1
        ctrller.redHat.transform.localScale=Vector3.zero;
        s.Join(DOTween.To(()=>{return Vector3.zero;},
            val=>{ctrller.redHat.transform.localScale=val;},
            Vector3.one, ctrller.a4_redHatShootDuration));
        //s.Join(ctrller.redHat.transform.DOScale(Vector3.one, redHatShootDuration));
        //shoot 3 bullets toward the player
        ctrller.redHat.animator.SetInteger("to_throw",3);
    }
}
