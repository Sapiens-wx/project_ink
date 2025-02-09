using System.Collections.Generic;
using UnityEngine;

public abstract class MobBase : EnemyBase
{
    [SerializeField] bool hatredPersistent;
    [Header("Distance Detection")]
    public float detectDist;
    public float groupDetectDist;
    [Header("chase")]
    public float chaseSpd;

    [HideInInspector] public bool playerInDetect, prevPlayerInDetect, playerInAttack, prevPlayerInAttack;
    internal virtual void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, detectDist);
        Gizmos.color=Color.black;
        Gizmos.DrawWireSphere(transform.position, groupDetectDist);
    }
    internal virtual void FixedUpdate(){
        //detection
        prevPlayerInDetect=playerInDetect;
        if(!hatredPersistent || !playerInDetect){
            playerInDetect=PlayerInRange(detectDist);
            if(playerInDetect&&!prevPlayerInDetect){ //on detect enter
                playerInDetect=false; //this is because i have to initiate the SetDetectPlayer function, but the function returns if playerInDetect is already set to true. wrote this line on purpose
                SetDetectPlayer();
            } else if(!playerInDetect&&prevPlayerInDetect) //on detect exit
                animator.SetBool("b_detect",false);
        }
    }
    public void SetDetectPlayer(){
        if(playerInDetect) return;
        playerInDetect=true;
        animator.SetBool("b_detect", true);
        List<MobBase> enemiesInRange=RoomManager.CurrentRoom.MobsInRange(transform.position, groupDetectDist);
        foreach(MobBase mob in enemiesInRange){
            if(mob.playerInDetect) continue;
            mob.SetDetectPlayer();
        }
    }
    public override void OnHit(Projectile proj)
    {
        base.OnHit(proj);
        SetDetectPlayer();
    }
}
