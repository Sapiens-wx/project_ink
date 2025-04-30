using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Rain : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatShootDist, redHatShootDuration;
    [SerializeField] float bullet_y_spd;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action(animator);
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

    void Action(Animator animator){
        ctrller.redHat.transform.position=ctrller.transform.position;
        ctrller.redHat.SetActive(true);
        Sequence s = DOTween.Sequence();
        //shoot redhat upward
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.redHat.transform.position.y+redHatShootDist, redHatShootDuration));
        //shoot 3 bullets toward the player
        s.AppendCallback(()=>{
            //animate the redhat
            ctrller.redHatAnimator.SetTrigger("to_throwUp");
            for(int i=0;i<3;++i){
                //intantiate bullet
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet(EnemyBulletManager.inst.boss1_a5);
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
        });
        //redhat returns
        s.Append(ctrller.redHat.transform.DOMove(ctrller.transform.position, redHatShootDuration));
        s.AppendCallback(()=>ctrller.redHat.SetActive(false));
        //return to idle state
        s.AppendCallback(()=>animator.SetTrigger("toIdle"));
    }
}
