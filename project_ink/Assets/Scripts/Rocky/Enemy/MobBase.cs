using UnityEngine;

public abstract class MobBase : EnemyBase
{
    [Header("Detection")]
    public float detectDist;
    [Header("Attack")]
    public float attackTriggerDist;

    [HideInInspector] public bool playerInDetect, prevPlayerInDetect, playerInAttack, prevPlayerInAttack;
    internal virtual void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, detectDist);
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDist);
    }
    internal virtual void FixedUpdate(){
        //detection
        prevPlayerInDetect=playerInDetect;
        playerInDetect=PlayerInRange(detectDist);
        if(playerInDetect&&!prevPlayerInDetect){ //on detect enter
            animator.SetBool("b_detect", true);
        } else if(!playerInDetect&&prevPlayerInDetect) //on detect exit
            animator.SetBool("b_detect",false);
        //attack
        prevPlayerInAttack=playerInAttack;
        playerInAttack=PlayerInRange(attackTriggerDist);
        if(playerInAttack&&!prevPlayerInAttack){ //on detect enter
            animator.SetBool("b_attack", true);
        } else if(!playerInAttack&&prevPlayerInAttack) //on detect exit
            animator.SetBool("b_attack",false);
    }
    public override void OnHit(Projectile proj)
    {
        base.OnHit(proj);
        animator.SetBool("b_detect", true);
    }
}
