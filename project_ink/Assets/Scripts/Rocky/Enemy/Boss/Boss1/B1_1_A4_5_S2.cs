using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_A4_5_S2 : StateBase<B1_1_Ctrller>
{
    [SerializeField] float restTime;
    [Header("Action 4")]
    [SerializeField] float bulletSpd;
    [SerializeField] float shootInterval;
    [Header("Action 5")]
    [SerializeField] float bullet_y_spd;

    bool isRedHatActing;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        isRedHatActing=true;

        Sequence s = DOTween.Sequence();
        s.AppendInterval(restTime);
        s.AppendCallback(()=>{
            if(isRedHatActing) Debug.LogError("[Action 4.5 Stage 2] Boss1_1's rest time is shorter than redhat's attack duration. this may cause error.");
            animator.SetTrigger("toIdle");
            });
        //randomly choose an attack for the redHat
        if(Random.Range(0,2)==0) Action1();
        else Action2();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    void Action1(){
        Sequence s = DOTween.Sequence();
        //shoot 3 bullets toward the player
        for(int i=0;i<3;++i){
            s.AppendCallback(()=>{
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet(EnemyBulletManager.inst.boss1_a4);
                bullet.transform.position=ctrller.redHat.transform.position;
                bullet.rgb.velocity=((Vector2)PlayerShootingController.inst.transform.position-(Vector2)bullet.transform.position).normalized*bulletSpd;
            });
            if(i!=2) s.AppendInterval(shootInterval);
        }
        s.AppendCallback(()=>isRedHatActing=false);
    }
    void Action2(){
        //shoot 3 bullets toward the player
        for(int i=0;i<3;++i){
            //intantiate bullet
            EnemyBulletBase bullet=Instantiate(EnemyBulletManager.inst.boss1_a5);
            bullet.transform.position=ctrller.redHat.transform.position;
            //x position of the bullet when hit the ground
            float vy=bullet_y_spd, g=bullet.rgb.gravityScale*9.8f, y1=RoomManager.inst.RoomBounds.min.y;
            float randBulletX=Random.Range(RoomManager.inst.RoomBounds.min.x, RoomManager.inst.RoomBounds.max.x);
            float distx=randBulletX-bullet.transform.position.x;
            float t1=vy/g;
            float h0=vy*t1-0.5f*g*t1*t1;
            float h=bullet.transform.position.y+h0-y1;
            float t2=Mathf.Sqrt(2*h/g);
            float t=t1+t2;
            float vx=distx/t;
            bullet.rgb.velocity=new Vector2(vx,vy);
        }
        isRedHatActing=false;
    }
}
