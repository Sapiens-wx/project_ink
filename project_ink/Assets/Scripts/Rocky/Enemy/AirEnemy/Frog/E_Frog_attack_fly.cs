using UnityEngine;

public class E_Frog_attack_fly : StateBase<E_Frog>{
    float waitUntil;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        waitUntil=Time.time+ctrller.waitDuration;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 v=ctrller.rgb.velocity;
        v.y=Mathf.Sin(Time.time);
        ctrller.rgb.velocity=v;
        if(Time.time>=waitUntil) animator.SetTrigger("attack");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        ctrller.rgb.velocity=new Vector2(ctrller.rgb.velocity.x,0);
    }
}