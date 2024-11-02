using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Dash_S2 : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatShootDist, redHatShootDuration;
    [SerializeField] float redHatToPlatformWaitTime, redHatToBossDuration;
    [SerializeField] float redHatDashDuration, dashDuration;
    [SerializeField] float waitTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
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
        Vector2 dir=PlayerShootingController.inst.transform.position-ctrller.redHat.transform.position;
        dir.Normalize();
        Vector2 targetPos=(Vector2)PlayerShootingController.inst.transform.position+dir*.3f;
        //create sequence
        Sequence s=DOTween.Sequence();
        //move the redhat to the boss
        s.Append(ctrller.redHat.transform.DOMove(ctrller.transform.position, redHatToBossDuration));
        //shoot the redhat
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.transform.position.y+redHatShootDist, redHatShootDuration));
        //move the redhat toward the player
        s.Append(ctrller.redHat.transform.DOMove(targetPos, redHatDashDuration));
        //wait for 1 sec
        s.AppendInterval(waitTime);
        //dash toward redhat
        s.Append(ctrller.transform.DOMove(targetPos, dashDuration));
        //set the marker(target)
        s.AppendCallback(()=>{
            //choose a random platform to place the marker(target)
            ctrller.a3_target.transform.position=Random.Range(0,2)==0?ctrller.st_platform1:ctrller.st_platform2;
            ctrller.a3_target.SetActive(true);
        });
        s.AppendInterval(redHatToPlatformWaitTime);
        //redhat goes to the target
        s.AppendCallback(()=>{
            ctrller.redHat.transform.position=ctrller.a3_target.transform.position;
            ctrller.a3_target.SetActive(false);
            animator.SetTrigger("toIdle");
            });
    }
}
