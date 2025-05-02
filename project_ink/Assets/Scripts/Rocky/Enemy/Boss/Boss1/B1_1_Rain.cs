using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Rain : StateBase<B1_1_Ctrller>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action();
    }
    void Action(){
        ctrller.redHat.transform.position=ctrller.transform.position;
        ctrller.redHat.gameObject.SetActive(true);
        Sequence s = DOTween.Sequence();
        //shoot redhat upward
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.redHat.transform.position.y+ctrller.a4_redHatShootDist, ctrller.a4_redHatShootDuration));
        //scale the redHat from 0 to 1
        Vector3 toScale=Vector3.one;
        if(PlayerCtrl.inst.transform.position.x>ctrller.transform.position.x)
            toScale.x=-1;
        ctrller.redHat.transform.localScale=Vector3.zero;
        s.Join(DOTween.To(()=>{return Vector3.zero;},
            val=>{ctrller.redHat.transform.localScale=val;},
            toScale, ctrller.a4_redHatShootDuration));
        //shoot 3 bullets toward the player
        s.AppendCallback(()=>ctrller.redHat.animator.SetTrigger("to_throwUp"));
    }
}
