using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Pidle : PStateBase
{
    Coroutine coro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro = player.StartCoroutine(m_FixedUpdate());
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (coro != null) {
            player.StopCoroutine(coro);
            coro=null;
        }
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            //switch to run
            if(player.inputx!=0 && player.onGround) player.animator.SetTrigger("run");
            //update dir
            player.UpdateDir();
            //jump
            Jump();
            //dash
            Dash();
            //apply gravity
            ApplyGravity();
            yield return wait;
        }
    }
    override internal void ApplyGravity(){
        if(player.onGround){
            if(!player.prevOnGround && player.v.y<0) //on ground enter
                player.v.y=0;
        }
        else if(player.v_trap.y<=0){ //if the player jumps up by trap_spring, avoid settrigger(jump_down)
            player.animator.SetTrigger("jump_down");
        }
    }
}
