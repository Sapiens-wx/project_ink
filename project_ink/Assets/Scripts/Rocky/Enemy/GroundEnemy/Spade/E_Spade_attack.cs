using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Spade_attack : StateBase<E_Spade>
{
    Coroutine dashCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        dashCoro=ctrller.StartCoroutine(Dash());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        dashCoro=StopCoroutineIfNull(dashCoro);
    }
    IEnumerator Dash(){
        float dashSpd=ctrller.dashDist/ctrller.dashInterval;
        ctrller.UpdateDir();
        ctrller.rgb.velocity=new Vector2(ctrller.Dir==1?dashSpd:-dashSpd, 0);
        yield return new WaitForSeconds(ctrller.dashInterval);
        ctrller.rgb.velocity=Vector2.zero;
        ctrller.animator.SetTrigger("attack_end");
    }
}
