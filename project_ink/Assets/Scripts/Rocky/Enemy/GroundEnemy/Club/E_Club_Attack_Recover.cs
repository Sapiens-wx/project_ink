using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class E_Club_Attack_Recover : StateBase<E_Club>
{
    Coroutine endRecoverCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        endRecoverCoro=ctrller.StartCoroutine(EndRecover());
        ctrller.rgb.velocity=Vector2.zero;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        endRecoverCoro=StopCoroutineIfNull(endRecoverCoro);
    }
    IEnumerator EndRecover(){
        yield return new WaitForSeconds(ctrller.attackRecoverInterval);
        ctrller.animator.SetTrigger("idle");
        endRecoverCoro=null;
    }
}
