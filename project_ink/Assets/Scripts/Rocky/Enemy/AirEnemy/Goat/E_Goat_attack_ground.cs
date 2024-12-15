using UnityEngine;

public class E_Goat_attack_ground : StateBase<E_Goat_Ground>{
    float cliffPosX;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.Dir=(int)Mathf.Sign(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x);
        ctrller.rgb.velocity=new Vector2(ctrller.Dir==1?ctrller.groundDashSpd:-ctrller.groundDashSpd,0);
        cliffPosX=ctrller.Dir==1?ctrller.patrol_xmax:ctrller.patrol_xmin;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Mathf.Abs(ctrller.transform.position.x-cliffPosX)<.04f) animator.SetTrigger("idle");
    }
}