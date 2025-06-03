using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Witch_Attack : StateBase<E_Witch>
{
    const float summonDist = 2f;
    const float ac2_flyDuration=1.7f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //random action
        int rand;
        if(RoomManager.inst.HasEnemy(e=>{return e as E_Monkey!=null || e as E_Frog!=null || e as E_Goat_Ground!=null;}))
            rand=Random.Range(0,2);
        else
            rand=Random.Range(0,3);
        switch(rand){
            case 0: //action 1
                ctrller.StartCoroutine(Action1());
                break;
            case 1: //action 2
                ctrller.StartCoroutine(Action2());
                break;
            case 2: //action 3
                ctrller.StartCoroutine(Action3());
                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    IEnumerator Action1(){
        //move until player is in attack
        WaitForSeconds wait=new WaitForSeconds(.3f);
        while(!ctrller.playerInAttack){
            ctrller.Dir=(int)Mathf.Sign(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x);
            ctrller.rgb.velocity=new Vector2(ctrller.Dir==1?ctrller.chaseSpd:-ctrller.chaseSpd,0);
            yield return wait;
        }
        ctrller.rgb.velocity=Vector2.zero;
        //charge
        yield return new WaitForSeconds(ctrller.ac1_chargeTime);

        Vector2 dir=((Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.transform.position).normalized;
        float cos=Mathf.Cos(Mathf.PI/6), sin=Mathf.Sin(Mathf.PI/6); //cos(PI/6) and sin(PI/6). 30 degree
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.witch, ctrller.transform.position, new Vector2(cos*dir.x-sin*dir.y, sin*dir.x+cos*dir.y));
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.witch, ctrller.transform.position, new Vector2(cos*dir.x+sin*dir.y, -sin*dir.x+cos*dir.y));
        EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.witch, ctrller.transform.position, dir);
        ctrller.animator.SetTrigger("idle");
    }
    //
    IEnumerator Action2(){
        Bounds roomBounds=RoomManager.CurrentRoom.RoomBounds;
        float dest;
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        if(ctrller.transform.position.x<roomBounds.center.x){ //closer to the left
            dest=roomBounds.min.x+ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
            ctrller.rgb.velocity=new Vector2(-ctrller.chaseSpd,0);
            while(ctrller.transform.position.x>dest)
                yield return wait;
            ctrller.rgb.velocity=Vector2.zero;
            yield return new WaitForSeconds(ctrller.ac2_chargeTime);
            ctrller.rgb.velocity=new Vector2(roomBounds.size.x/ac2_flyDuration,0);
            for(int i=0;i<3;++i){
                yield return new WaitForSeconds(0.5f);
                EnemyBulletManager.InstantiateBullet_v(EnemyBulletManager.inst.witch, ctrller.transform.position, Vector2.zero).gravity=9.8f;
            }
            yield return new WaitForSeconds(ac2_flyDuration-1.5f);
        }
        else{ //close to the right
            dest=roomBounds.max.x-ctrller.bc.bounds.extents.x+ctrller.bc.offset.x;
            ctrller.rgb.velocity=new Vector2(ctrller.chaseSpd,0);
            while(ctrller.transform.position.x<dest)
                yield return wait;
            ctrller.rgb.velocity=Vector2.zero;
            yield return new WaitForSeconds(ctrller.ac2_chargeTime);
            ctrller.rgb.velocity=new Vector2(-roomBounds.size.x/ac2_flyDuration,0);
            for(int i=0;i<3;++i){
                yield return new WaitForSeconds(0.5f);
                EnemyBulletManager.InstantiateBullet_v(EnemyBulletManager.inst.witch, ctrller.transform.position, Vector2.zero).gravity=9.8f;
            }
            yield return new WaitForSeconds(ac2_flyDuration-1.5f);
        }
        ctrller.animator.SetTrigger("idle");
    }
    IEnumerator Action3(){
        EnemyType enemyType=(EnemyType)Random.Range((int)EnemyType.Frog, (int)EnemyType.Monkey+1);
        MobBase enemy = (MobBase)RoomManager.inst.InstantiateEnemy(enemyType);
        enemy.transform.position=ctrller.transform.position;
        enemy.ToTheGround();
        //give hatred to the spawned enemies
        enemy.SetDetectPlayer();

        ctrller.animator.SetTrigger("idle");
        yield break;
    }
}
