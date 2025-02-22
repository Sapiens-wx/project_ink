using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Prun : PStateBase
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
        player.StopCoroutine(coro);
        coro=null;
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            Movement();
            Jump();
            Dash();
            ApplyGravity();
            yield return wait;
        }
    }
    override internal void Movement(){
        player.v.x=player.xspd*player.inputx;
        //change direction
        if(player.inputx==0){
            player.animator.SetTrigger("idle");
        }
        else if(player.inputx!=-player.dir){
            player.Dir=-player.inputx;
        }
    }
    override internal void Jump(){
        if(player.onGround && Time.time-player.jumpKeyDown<=player.coyoteTime){
            player.jumpKeyDown=-100;
            player.animator.SetTrigger("jump_up");
        }
    }
    override internal void ApplyGravity(){
        if(player.onGround){
            if(!player.prevOnGround && player.v.y<0) //on ground enter
                player.v.y=0;
        }
        else{
            player.animator.SetTrigger("jump_down");
        }
    }
}
