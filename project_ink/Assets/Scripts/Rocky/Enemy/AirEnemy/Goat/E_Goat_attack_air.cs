using UnityEngine;

public class E_Goat_attack_air : StateBase<E_Goat_Ground>{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Vector2 dir=PlayerShootingController.inst.transform.position-ctrller.transform.position;
        dir=dir.normalized*ctrller.airDashSpd;
        ctrller.rgb.velocity=dir;
        ctrller.isDashing=true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.isDashing=false;
    }
}