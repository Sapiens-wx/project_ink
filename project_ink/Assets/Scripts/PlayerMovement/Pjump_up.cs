using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using UnityEngine;

public class Pjump_up : PStateBase
{
    Coroutine coro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.jumpKeyUp=false;
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
        int j=player.jumpUpIgnoredColliders.Count;
        for(int i=0;i<j;){
            //if player is touching the platform when starts to jump down, then still ignore the collision
            if(!IsTouching(player.bc, player.jumpUpIgnoredColliders[i])){
                //cancel collision ignore
                Physics2D.IgnoreCollision(player.bc, player.jumpUpIgnoredColliders[i], false);
                player.jumpUpIgnoredColliders[i]=player.jumpUpIgnoredColliders[--j];
            } else i++;
        }
        player.jumpUpIgnoredColliders.RemoveRange(j, player.jumpUpIgnoredColliders.Count-j);
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            player.UpdateDir();
            Movement();
            Jump();
            Dash();
            ApplyGravity();
            CeilingCheck();
            yield return wait;
        }
    }
    internal override void CeilingCheck(){
        Vector2 lt=(Vector2)player.transform.position+player.leftTop, rt=(Vector2)player.transform.position+player.rightTop;
        if(Physics2D.OverlapArea(lt, rt, GameManager.inst.groundLayer)){
            if(player.v.y>0){
                player.v.y=0;
                player.animator.SetTrigger("jump_down");
            } 
        }
        Collider2D platform=Physics2D.OverlapArea(player.bc.bounds.min, player.bc.bounds.max, GameManager.inst.platformLayer);
        if(platform) //ignore the collision of the platform to enable the player to go through the platform
            player.IgnoreCollision(platform, player.jumpUpIgnoredColliders);
    }
    override internal void ApplyGravity(){
        player.v.y+=player.gravity*Time.fixedDeltaTime;
        if(player.v.y<0) player.v.y=0;
    }
    override internal void Jump(){
        if(player.jumpKeyUp){
            player.jumpKeyUp=false;
            player.v.y=0;
        }
        if(player.v_trap.y<=0&&player.v.y<=0){
            player.jumpKeyUp=false;
            player.v.y=0;
            player.animator.SetTrigger("jump_down");
        }
    }
    bool IsTouching(Collider2D c1,Collider2D c2){
        if(c1 is BoxCollider2D && c2 is BoxCollider2D){
            Bounds cb1=c1.bounds,cb2=c2.bounds;
            return !(cb1.min.x>cb2.max.x||cb1.max.x<cb2.min.x||cb1.min.y>cb2.max.y||cb1.max.y<cb2.min.y);
        }
        return c1.IsTouching(c2);
    }
}
