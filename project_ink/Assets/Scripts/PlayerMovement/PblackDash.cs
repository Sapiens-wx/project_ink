using UnityEngine;

public class PblackDash : Pdash{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.hittable=false;
        player.blackDashTriggered=true;
        animator.SetBool("blackdash", false);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        base.OnStateExit(animator, stateInfo, layerIndex);
        player.hittable=true;
        player.canSetBlackDash=Time.time+player.blackDashCoolDown;
    }
}