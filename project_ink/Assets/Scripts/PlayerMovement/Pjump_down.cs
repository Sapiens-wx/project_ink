using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Pjump_down : PStateBase
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
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.StopCoroutine(coro);
        coro=null;
        player.ClearIgnoredCollision();
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            player.UpdateDir();
            Jump();
            Movement();
            Dash();
            ApplyGravity();
            yield return wait;
        }
    }
    override internal void ApplyGravity(){
        if(player.onGround && player.v.y<0){
            player.v.y=0;
            player.animator.SetTrigger("idle");
        }
        else if(player.v.y>=player.maxFallSpd)
            player.v.y+=player.gravity*Time.fixedDeltaTime;
    }
    override internal void Jump(){
        //the first detection is because of the coyote time: avoid double jump if the player left the ground by jumping
        if(Mathf.Abs(player.onGroundTime-player.secondToLastJumpKeyDown)>.2f && Time.time-player.onGroundTime<player.coyoteTime && Time.time-player.jumpKeyDown<=player.jumpBufferTime){
            player.jumpKeyDown=-100;
            player.animator.SetTrigger("jump_up");
        }
    }
}