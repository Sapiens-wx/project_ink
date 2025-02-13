using System.Collections.Generic;
using UnityEngine;

public abstract class MobBase : EnemyBase
{
    [SerializeField] bool hatredPersistent;
    [Header("Distance Detection")]
    public float loseHatredInterval;
    public float detectDist;
    public float groupDetectDist;
    [Header("chase")]
    public float chaseSpd;

    [HideInInspector] public bool inHatred, prevInHatred, playerInAttack, prevPlayerInAttack;
    internal bool playerInDetect, prevPlayerInDetect;
    float loseHatredTime;
    internal virtual void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, detectDist);
        Gizmos.color=Color.black;
        Gizmos.DrawWireSphere(transform.position, groupDetectDist);
    }
    internal override void Start()
    {
        base.Start();
        loseHatredTime=-1;
    }
    internal virtual void FixedUpdate(){
        //detection
        prevPlayerInDetect=playerInDetect;
        if(!hatredPersistent || !playerInDetect){
            playerInDetect=PlayerInRange(detectDist);
            if(playerInDetect&&!prevPlayerInDetect){ //on detect enter
                loseHatredTime=-1;
                prevInHatred=inHatred;
                inHatred=true;
                if(inHatred&&!prevInHatred){
                    inHatred=false; //this is because i have to initiate the SetDetectPlayer function, but the function returns if playerInDetect is already set to true. wrote this line on purpose
                    SetDetectPlayer();
                }
            } else if(!playerInDetect&&prevPlayerInDetect){ //on detect exit
                loseHatredTime=Time.time+loseHatredInterval;
            }
            if(inHatred&&loseHatredTime>0&&Time.time>=loseHatredTime){ //lose hatred
                loseHatredTime=-1;
                prevInHatred=inHatred;
                inHatred=false;
                animator.SetBool("b_detect",false);
            }
        }
    }
    public void SetDetectPlayer(){
        if(inHatred) return;
        inHatred=true;
        animator.SetBool("b_detect", true);
        List<MobBase> enemiesInRange=RoomManager.CurrentRoom.MobsInRange(transform.position, groupDetectDist);
        foreach(MobBase mob in enemiesInRange){
            if(mob.inHatred) continue;
            mob.SetDetectPlayer();
        }
    }
    public override void OnHit(Projectile proj)
    {
        base.OnHit(proj);
        SetDetectPlayer();
    }
}
