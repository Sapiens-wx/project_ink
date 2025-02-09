using System.Collections.Generic;
using UnityEngine;

public abstract class EliteBase_Air : EliteBase
{
    [Header("Attack Detection")]
    public Bounds attackTriggerBounds;
    [Header("Chase")]
    public float chaseSpd;

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
    }
}