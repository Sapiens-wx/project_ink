using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_StageTransit : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatShootDist, redHatShootDuration;
    [SerializeField] float moveToPlatformDuration;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action(animator);
    }

    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    void Action(Animator animator){
        ctrller.redHat.gameObject.SetActive(true);
        ctrller.redHat.transform.position=ctrller.transform.position;

        Sequence s=DOTween.Sequence();
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.transform.position.y+redHatShootDist, redHatShootDuration));
        //randomly move to a platform
        s.Append(ctrller.redHat.transform.DOMove(ctrller.OffsetPlatformPos(Random.Range(0,2)==0?ctrller.platform1:ctrller.platform2,ctrller.redHat.bc.bounds.extents.y), moveToPlatformDuration));
        s.AppendCallback(()=>{
            animator.SetTrigger("toIdle2");
        });
    }
}