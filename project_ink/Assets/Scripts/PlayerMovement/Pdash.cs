using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Pdash : PStateBase
{
    Coroutine dashCoro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.ResetTrigger("dash_recover");
        if(player.inputx==0)
            player.dashDir=-player.Dir;
        else{
            player.dashDir=player.inputx>0?1:-1;
            player.Dir=-player.dashDir;
        }
        dashCoro = player.StartCoroutine(DashAnim());

        PlayerEffects.inst.PlayEffect(PlayerEffects.EffectType.Dash);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(dashCoro!=null){
            player.StopCoroutine(dashCoro);
            dashCoro=null;
            player.v.x=0;
        }
    }
}
