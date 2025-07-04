using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PStateBase : StateMachineBehaviour
{
    internal PlayerCtrl player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(player==null) player=PlayerCtrl.inst;
    }
    internal void UpdateVelocity(){
        player.rgb.velocity=player.v;
    }
    virtual internal void ApplyGravity(){
        if(player.onGround){
            if(!player.prevOnGround && player.v.y<0) //on ground enter
                player.v.y=0;
        }//if player is not wall jumping, is on wall, and is pressing the button the opposite dir of [dir], then the player should cling on the wall
        else if(player.v.y>=player.maxFallSpd)
            player.v.y+=player.gravity*Time.fixedDeltaTime;
    }
    virtual internal void Movement(){
        player.v.x=player.xspd*player.inputx;
    }
    virtual internal void Dash(){
        //dash
        if(Time.time-player.dashKeyDown<=player.keyDownBuffTime && player.dashKeyDown+player.keyDownBuffTime>player.allowDashTime){
            player.allowDashTime=Time.time+player.dashInterval;
            player.dashKeyDown=-100;
            if(player.canDash){
                player.canDash=false;
                player.animator.SetTrigger("dash");
            }
        }
    }
    internal IEnumerator DashAnim(){
        player.v.y=0;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float dashSpd=player.dashDist/Time.fixedDeltaTime;
        for(int i=0;i<player.dashPercents.Length;++i){
            player.v.x=player.dashDir==1?player.dashPercents[i]*dashSpd:-player.dashPercents[i]*dashSpd;
            yield return wait;
        }
        player.animator.SetTrigger("dash_recover");
    }
    internal virtual void CeilingCheck(){
        if(Physics2D.OverlapArea((Vector2)player.transform.position+player.leftTop, (Vector2)player.transform.position+player.rightTop, GameManager.inst.groundLayer)){
            if(player.v.y>0){
                player.v.y=0;
                player.animator.SetTrigger("jump_down");
            } 
        }
    }
    protected virtual void JumpDown(){
        if(player.jumpDownKey){
            Collider2D[] cds=Physics2D.OverlapAreaAll((Vector2)player.transform.position+player.leftBot, (Vector2)player.transform.position+player.rightBot, GameManager.inst.platformLayer);
            if(cds!=null){
                foreach(Collider2D cd in cds)
                    player.IgnoreCollision(cd, player.jumpDownIgnoredColliders);
            }
        }
    }
    virtual internal void Jump(){
        if(Time.time-player.onGroundTime<player.coyoteTime && Time.time-player.jumpKeyDown<=player.jumpBufferTime){
            player.jumpKeyDown=-100;
            player.v.y=player.yspd;
            player.animator.SetTrigger("jump_up");
        }
    }
    internal IEnumerator InvincibleTimer(){
        player.hitAnim.Restart();
        yield return new WaitForSeconds(player.invincibleTime);
        player.hittable=true;
        player.hitAnim.Pause();
        player.spr.GetPropertyBlock(player.matPB);
        player.matPB.SetFloat("_whiteAmount",.5f);
        player.spr.SetPropertyBlock(player.matPB);
    }
}
