using UnityEngine;

public abstract class EnemyBase_Air : MobBase
{
    [Header("Attack Detection")]
    public float attackTriggerDist;
    [Header("Idle State")]
    //used for idle state 1
    public int restDir; //is it upside down or upside up. 1 for up and -1 for down
    //used for idle state 2
    public float idle2_radius, idle2_flyAngularSpd;

    public event System.Action<Collision2D> onCollisionEnter;
    internal override void OnDrawGizmosSelected(){
        base.OnDrawGizmosSelected();
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDist);
    }
    internal override void Start(){
        base.Start();
    }
    internal override void FixedUpdate()
    {
        base.FixedUpdate();
        //attack
        prevPlayerInAttack=playerInAttack;
        playerInAttack=PlayerInRange(attackTriggerDist);
        if(playerInAttack&&!prevPlayerInAttack){ //on detect enter
            animator.SetBool("b_attack", true);
        } else if(!playerInAttack&&prevPlayerInAttack) //on detect exit
            animator.SetBool("b_attack",false);
    }
    void OnCollisionEnter2D(Collision2D collision){
        onCollisionEnter?.Invoke(collision);
    }
}