using UnityEngine;

public abstract class MobBase : EnemyBase
{
    [SerializeField] bool hatredPersistent;
    [Header("Distance Detection")]
    public float detectDist;
    public float attackTriggerDist;
    [Header("chase")]
    public float chaseSpd;

    [HideInInspector] public bool playerInDetect, prevPlayerInDetect, playerInAttack, prevPlayerInAttack;
    internal virtual void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, detectDist);
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDist);
    }
    internal virtual void FixedUpdate(){
        //detection
        prevPlayerInDetect=playerInDetect;
        if(!hatredPersistent || !playerInDetect){
            playerInDetect=PlayerInRange(detectDist);
            if(playerInDetect&&!prevPlayerInDetect){ //on detect enter
                animator.SetBool("b_detect", true);
            } else if(!playerInDetect&&prevPlayerInDetect) //on detect exit
                animator.SetBool("b_detect",false);
        }
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
