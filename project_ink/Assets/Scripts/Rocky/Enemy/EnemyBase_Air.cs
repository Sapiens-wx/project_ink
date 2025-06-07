using UnityEngine;

public abstract class EnemyBase_Air : MobBase
{
    [Header("Attack Detection")]
    public float attackTriggerDist;
    public float attackRecoverTime;
    [Header("Idle State")]
    //used for idle state 1
    public int restDir; //is it upside down or upside up. 1 for up and -1 for down
    //used for idle state 2
    public float idle2_radius, idle2_flyAngularSpd;

    public event System.Action<Collision2D> onCollisionEnter;
    float nextAttackTime;
    bool animatorAttackBool, canAttack;
    bool AnimatorAttackBool{
        get=>animatorAttackBool;
        set{
            animatorAttackBool=value;
            animator.SetBool("b_attack", value);
        }
    }
    internal override void OnDrawGizmosSelected(){
        base.OnDrawGizmosSelected();
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDist);
    }
    internal override void Start(){
        base.Start();
        canAttack=false;
        animatorAttackBool=false;
        nextAttackTime=-1;
    }
    internal override void FixedUpdate()
    {
        base.FixedUpdate();
        //attack
        prevPlayerInAttack=playerInAttack;
        playerInAttack=PlayerInRange(attackTriggerDist);
        canAttack=playerInAttack&&Time.time>=nextAttackTime;
        if(canAttack!=AnimatorAttackBool){
            AnimatorAttackBool=canAttack;
            if(canAttack) nextAttackTime=Time.time+attackRecoverTime; //has attack recover time
        }
    }
    void OnCollisionEnter2D(Collision2D collision){
        onCollisionEnter?.Invoke(collision);
    }
}