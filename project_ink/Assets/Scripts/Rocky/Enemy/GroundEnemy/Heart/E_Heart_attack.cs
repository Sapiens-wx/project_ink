using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class E_Heart_attack : StateBase<E_Heart>{
    Coroutine recoverCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        recoverCoro=ctrller.StartCoroutine(Recover());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        recoverCoro=StopCoroutineIfNull(recoverCoro);
    }
    IEnumerator Recover(){
        List<EnemyBase> enemiesInRange=RoomManager.inst.EnemiesInRange(ctrller.transform.position, ctrller.recoverRange);
        WaitForSeconds wait=new WaitForSeconds(1);
        float recoverEndTime=Time.time+ctrller.recoverDuration;
        while(Time.time<recoverEndTime){
            foreach(EnemyBase e in enemiesInRange)
                e.CurHealth+=ctrller.recoverAmount;
            yield return wait;
        }
        recoverCoro=null;
        ctrller.animator.SetTrigger("idle");
    }
}