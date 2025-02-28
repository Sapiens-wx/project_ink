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
        player.v.y=player.yspd;
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
        int j=player.ignoredColliders.Count;
        for(int i=0;i<j;){
            //if player is touching the platform when starts to jump down, then still ignore the collision
            if(!IsTouching(player.bc, player.ignoredColliders[i])){
                Debug.Log($"player is not touching {player.ignoredColliders[i].name}");
                //cancel collision ignore
                Physics2D.IgnoreCollision(player.bc, player.ignoredColliders[i], false);
                player.ignoredColliders[i]=player.ignoredColliders[--j];
            } else i++;
        }
        player.ignoredColliders.RemoveRange(j, player.ignoredColliders.Count-j);
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
        Collider2D platform=Physics2D.OverlapArea(lt, rt, GameManager.inst.platformLayer);
        if(platform) //ignore the collision of the platform to enable the player to go through the platform
            player.IgnoreCollision(platform);
    }
    override internal void ApplyGravity(){
        player.v.y+=player.gravity*Time.fixedDeltaTime;
    }
    override internal void Jump(){
        if(player.v.y<=0 || player.jumpKeyUp){
            player.jumpKeyUp=false;
            player.v.y=0;
            player.animator.SetTrigger("jump_down");
        }
    }
    bool IsTouching(Collider2D c1,Collider2D c2){
        return c1.bounds.Intersects(c2.bounds);
    }
}
