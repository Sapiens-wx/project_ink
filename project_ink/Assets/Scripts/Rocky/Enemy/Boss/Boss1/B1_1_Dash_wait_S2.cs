using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_wait_S2 : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatShootWaitTime, waitTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.UpdateDir();
        Action(animator);
    }

    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}
    void Action(Animator animator){
        //calculate direction
        Vector2 dir=PlayerCtrl.inst.transform.position-ctrller.transform.position;
        dir.Normalize();
        Vector2 targetPos=(Vector2)PlayerShootingController.inst.transform.position+dir*.3f;
        float zrotation=Vector2.SignedAngle(Vector2.right,dir);
        if(zrotation>90||zrotation<-90)
            zrotation-=180;
        //create sequence
        Sequence s=DOTween.Sequence();
        //wait
        s.AppendInterval(redHatShootWaitTime);
        //shoot the redhat
        s.AppendCallback(()=>ctrller.redHat.transform.position=targetPos);
        //wait for 1 sec
        s.AppendInterval(waitTime);
        s.AppendCallback(()=>animator.SetTrigger("toIdle"));
    }
}
