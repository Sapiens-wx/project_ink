using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Idle : StateBase<Tentacle>
{
    Coroutine coro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro=ctrller.StartCoroutine(m_FixedUpdate());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(coro!=null){
            ctrller.StopCoroutine(coro);
            coro=null;
        }
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            if(ctrller.GetAttackCount()>0){
                ctrller.animator.SetInteger("index", ctrller.GetPeekAttackAnimIdx());
                ctrller.animator.SetTrigger("attack");
            }
            yield return wait;
        }
    }
}
