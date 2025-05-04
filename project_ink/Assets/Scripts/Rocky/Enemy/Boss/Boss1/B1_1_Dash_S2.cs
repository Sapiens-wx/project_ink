using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_S2 : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatToPlatformWaitTime;
    [SerializeField] float dashDuration;
    [SerializeField] float redHatShootWaitTime, waitTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.UpdateDir();
        Action2(animator);
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

    void Action2(Animator animator){
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
        //adjust rotation while dash
        s.Append(ctrller.transform.DORotate(new Vector3(0,0,zrotation),.3f));
        //dash toward redhat
        s.Append(ctrller.transform.DOMove(targetPos, dashDuration));
        //readjust rotation after dash
        s.Append(ctrller.transform.DORotate(Vector3.zero,.3f));
        //set the marker(target)
        s.AppendCallback(()=>{
            //choose a random platform to place the marker(target)
            ctrller.a3_target.transform.position=ctrller.OffsetPlatformPos(Random.Range(0,2)==0?ctrller.platform1:ctrller.platform2,1);
            ctrller.a3_target.SetActive(true);
        });
        s.Append(ctrller.redHat.transform.DOScale(Vector3.zero,.5f));
        s.AppendInterval(redHatToPlatformWaitTime);
        //redhat goes to the target
        s.AppendCallback(()=>{
            ctrller.redHat.transform.localScale=Vector3.one;
            ctrller.redHat.transform.position=ctrller.a3_target.transform.position;
            ctrller.a3_target.SetActive(false);
            animator.SetTrigger("toIdle");
            });
    }
}
