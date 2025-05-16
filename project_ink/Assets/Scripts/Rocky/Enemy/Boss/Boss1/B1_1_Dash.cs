using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash: StateBase<B1_1_Ctrller>
{
    public float dashDuration;
    public float startDisappearDist;
    public float disappearDuration;
    float disappearDistSqr;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Teleport(animator);
        disappearDistSqr=startDisappearDist*startDisappearDist;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 dir=(Vector2)ctrller.transform.position-(Vector2)ctrller.redHat.transform.position;
        float distSqr = Vector2.Dot(dir,dir);
        //redhat disappear animation
        if(distSqr<disappearDistSqr){
            disappearDistSqr=-1;
            ctrller.redHat.transform.DOScale(Vector3.zero, disappearDuration);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    void Teleport(Animator animator){
        Vector2 targetPos=ctrller.redHat.transform.position;
        Vector2 dir=targetPos-(Vector2)ctrller.transform.position;
        float zrotation=Vector2.SignedAngle(Vector2.right,dir);
        if(zrotation>90||zrotation<-90)
            zrotation-=180;
        //create sequence
        Sequence s=DOTween.Sequence();
        //adjust rotation while dash
        s.Append(ctrller.transform.DORotate(new Vector3(0,0,zrotation),.3f));
        //dash toward redhat
        s.Append(ctrller.transform.DOMove(targetPos, dashDuration));
        ctrller.redHat.transform.localScale=Vector3.one;
        s.AppendCallback(()=>{
            ctrller.redHat.gameObject.SetActive(false);
            ctrller.redHat.transform.localScale=Vector3.one;
            animator.SetTrigger("toIdle");
            });
        s.Append(ctrller.transform.DORotate(Vector3.zero,.3f));
    }
}
