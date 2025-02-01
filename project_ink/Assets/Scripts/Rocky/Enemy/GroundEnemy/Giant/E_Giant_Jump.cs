using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class E_Giant_Jump : StateBase<E_Giant>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //on the left
        if(ctrller.rgb.position.x<PlayerShootingController.inst.transform.position.x){
            ctrller.rgb.velocity=MathUtil.CalcJumpVelocity(ctrller.rgb.position.x, PlayerShootingController.inst.transform.position.x-ctrller.attackRange, ctrller.jumpHeight, ctrller.rgb.gravityScale*9.8f);
        } else
            ctrller.rgb.velocity=MathUtil.CalcJumpVelocity(ctrller.rgb.position.x, PlayerShootingController.inst.transform.position.x+ctrller.attackRange, ctrller.jumpHeight, ctrller.rgb.gravityScale*9.8f);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if(ctrller.onGround && ctrller.rgb.velocity.y<.05f){
            animator.SetTrigger("attack");
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ctrller.rgb.velocity=Vector2.zero;
    }
}
