using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_S2 : StateBase<B1_1_Ctrller>
{
    [SerializeField] float dashDuration;
    public float startDisappearDist, disappearDuration;
    float disappearDistSqr;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        disappearDistSqr=startDisappearDist*startDisappearDist;
        Action2(animator);
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

    void Action2(Animator animator){
        //calculate direction
        Vector2 targetPos=ctrller.redHat.transform.position;
        Vector2 dir=targetPos-(Vector2)ctrller.transform.position;
        dir.Normalize();
        float zrotation=Vector2.SignedAngle(Vector2.right,dir);
        if(zrotation>90||zrotation<-90)
            zrotation-=180;
        //create sequence
        Sequence s=DOTween.Sequence();
        //adjust rotation while dash
        s.Append(ctrller.transform.DORotate(new Vector3(0,0,zrotation),.3f));
        //dash toward redhat
        s.Append(ctrller.transform.DOMove(targetPos, dashDuration));
        //readjust rotation after dash
        s.Append(ctrller.transform.DORotate(Vector3.zero,.3f));
        s.AppendCallback(() => animator.SetTrigger("toIdle"));
    }
}
