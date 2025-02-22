using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Pdash : PStateBase
{
    Coroutine coro;
    Coroutine dashCoro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.ResetTrigger("dash_recover");
        if(player.dashDir!=0){
            player.Dir=player.dashDir;
            player.dashDir=0;
        }
        else if(player.inputx==player.Dir) //change direction of dash
            player.Dir=-player.inputx;
        coro = player.StartCoroutine(m_FixedUpdate());
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
        player.StopCoroutine(coro);
        coro=null;
        if(dashCoro!=null){
            player.StopCoroutine(dashCoro);
            dashCoro=null;
            player.v.x=0;
        }
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            yield return wait;
        }
    }
}
