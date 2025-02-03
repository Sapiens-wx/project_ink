using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class E_Club_Attack : StateBase<E_Club>
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
        Vector2 v=new Vector2(ctrller.Dir==1?ctrller.attackDashSpd:-ctrller.attackDashSpd,0);
        v.y=ctrller.rgb.velocity.y;
        v.y=ctrller.rgb.velocity.y;
        ctrller.rgb.velocity=v;
        ctrller.rgb.velocity=v;
        float dashToTime=Time.time+ctrller.attackDashDist/ctrller.attackDashSpd;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(Time.time<dashToTime){
            yield return wait;
            v.y=ctrller.rgb.velocity.y;
            ctrller.rgb.velocity=v;
        }
        ctrller.animator.SetTrigger("attack_recover");
        dashCoro=null;
    }
}
