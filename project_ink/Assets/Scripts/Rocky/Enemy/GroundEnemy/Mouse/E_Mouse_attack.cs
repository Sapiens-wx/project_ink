using System.Collections;
using UnityEngine;


public class E_Mouse_attack : StateBase<E_Mouse>
{
    Coroutine coro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro = ctrller.StartCoroutine(SelectAttackPos());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        coro=StopCoroutineIfNull(coro);
    }
    IEnumerator SelectAttackPos(){
        WaitForSeconds wait=new WaitForSeconds(.3f);
        RaycastHit2D hit=Physics2D.Raycast(PlayerShootingController.inst.transform.position, Vector2.down, float.MaxValue, GameManager.inst.groundMixLayer);
        while(!hit){
            yield return wait;
            hit=Physics2D.Raycast(PlayerShootingController.inst.transform.position, Vector2.down, float.MaxValue, GameManager.inst.groundMixLayer);
        }
        Vector2 pos=hit.point;
        pos.y+=ctrller.bc.bounds.extents.y+ctrller.bc.offset.y;
        pos.x+=Random.Range(-ctrller.attackDist/2, ctrller.attackDist/2);
        ctrller.transform.position=pos;
        ctrller.animator.SetTrigger("attack");
        coro=null;
    }
}