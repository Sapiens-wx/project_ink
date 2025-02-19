using System.Collections.Generic;
using UnityEngine;

public abstract class EliteBase_Ground : EliteBase
{
    [Header("Chase")]
    public float chaseSpd;
    [Header("Attack Detection")]
    public Bounds attackTriggerBounds;

    [HideInInspector] public bool playerInAttack, prevPlayerInAttack;
    //ground detection
    [HideInInspector] public bool onGround, prevOnGround;
    internal virtual void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(attackTriggerBounds.center+transform.position, attackTriggerBounds.size);
    }
    internal virtual void FixedUpdate(){
        //attack trigger detection
        prevPlayerInAttack=playerInAttack;
        playerInAttack=PlayerInRange(attackTriggerBounds);
        if(playerInAttack&&!prevPlayerInAttack){ //on detect enter
            animator.SetBool("b_attack", true);
        } else if(!playerInAttack&&prevPlayerInAttack) //on detect exit
            animator.SetBool("b_attack",false);

        CheckOnGround();
        if(!prevOnGround && onGround){ //landing
            animator.SetBool("b_onground", true);
        } else if(!onGround && prevOnGround) //leave the ground
            animator.SetBool("b_onground", false);
    }
    internal void CheckOnGround(){
        prevOnGround=onGround;
        Bounds bounds=bc.bounds;
        Vector2 leftBot=bounds.min;
        Vector2 rightBot=leftBot;
        rightBot.x+=bounds.size.x*.9f;
        leftBot.x+=bounds.size.x*.1f;
        onGround = Physics2D.OverlapArea(leftBot,rightBot,GameManager.inst.groundLayer);
    }
}

public abstract class EliteBase : EnemyBase{
    /// <summary>
    /// activates the enemy (called when player enters the room)
    /// </summary>
    public void Activate(){
        animator.SetTrigger("chase");
        Debug.Log("set trigger chase");
    }
}