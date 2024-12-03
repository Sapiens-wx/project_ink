using System.Collections;
using UnityEngine;


public class E_Club_Attack_Anticipate : StateBase<E_Club>
{
    Coroutine toAttackCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        toAttackCoro=ctrller.StartCoroutine(ToAttackState());
        ctrller.rgb.velocity=Vector2.zero;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        toAttackCoro=StopCoroutineIfNull(toAttackCoro);
    }
    IEnumerator ToAttackState(){
        yield return new WaitForSeconds(ctrller.attackChargeInterval);
        ctrller.animator.SetTrigger("attack");
        toAttackCoro=null;
    }
}
