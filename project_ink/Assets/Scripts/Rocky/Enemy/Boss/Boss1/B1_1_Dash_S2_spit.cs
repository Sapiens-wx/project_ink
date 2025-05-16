using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_S2_spit : StateBase<B1_1_Ctrller>
{
    public float redHatDashDuration;
    /// <summary>
    /// when in normalized time of this clip should the redhat be thrown
    /// </summary>
    public float spitTimeNormalized;
    bool spitted;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        spitted=false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //spit the redhat
        if(spitted==false && stateInfo.normalizedTime>=spitTimeNormalized){
            spitted=true;
            Vector2 target=ctrller.OffsetPlatformPos(Random.Range(0,2)==0?ctrller.platform1:ctrller.platform2,1);
            //update boss's direction
            ctrller.Dir = target.x > ctrller.transform.position.x ? 1 : -1;
            //reset redHat's position
            ctrller.redHat.transform.position=ctrller.transform.position;
            ctrller.redHat.gameObject.SetActive(true);
            ctrller.redHat.transform.localScale=Vector3.one;
            ctrller.redHat.animator.SetTrigger("to_fly");
            //calculate direction
            Vector2 dir=target-(Vector2)ctrller.redHat.transform.position;
            dir.Normalize();
            //create sequence
            Sequence s=DOTween.Sequence();
            //move the redhat toward the player
            s.Append(ctrller.redHat.transform.DOMove(target, redHatDashDuration));
            s.AppendCallback(()=>{
                ctrller.redHat.animator.SetTrigger("to_idle");
                });
            //make redHat rotate toward the player
            ctrller.redHat.transform.eulerAngles=new Vector3(0,0,Vector2.SignedAngle(Vector2.up, dir));
            s.Append(ctrller.redHat.transform.DORotate(Vector3.zero, .2f));//rotate back
        }
    }
}