using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Shoot : StateBase<B1_1_Ctrller>
{
    [SerializeField] float redHatShootDist, redHatShootDuration;
    [SerializeField] float shootInterval;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Action1(animator);
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

    void Action1(Animator animator){
        ctrller.redHat.transform.position=ctrller.transform.position;
        ctrller.redHat.SetActive(true);
        Sequence s = DOTween.Sequence();
        //shoot redhat upward
        s.Append(ctrller.redHat.transform.DOMoveY(ctrller.redHat.transform.position.y+redHatShootDist, redHatShootDuration));
        //scale the redHat from 0 to 1
        ctrller.redHat.transform.localScale=Vector3.zero;
        Debug.Log(ctrller.redHat.transform.localScale);
        s.Join(DOTween.To(()=>{return Vector3.zero;},
            val=>{ctrller.redHat.transform.localScale=val; Debug.Log("set scale="+ctrller.redHat.transform.localScale);},
            Vector3.one, redHatShootDuration));
        //s.Join(ctrller.redHat.transform.DOScale(Vector3.one, redHatShootDuration));
        //shoot 3 bullets toward the player
        for(int i=0;i<3;++i){
            //animate the redhat
            ctrller.redHatAnimator.SetTrigger("to_throw");
            s.AppendCallback(()=>{
                Vector2 dir=((Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.redHat.transform.position).normalized;
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a4, ctrller.redHat.transform.position, dir);
            });
            if(i!=2) s.AppendInterval(shootInterval);
        }
        s.AppendCallback(()=>ctrller.redHatAnimator.SetTrigger("to_idle"));
        //redhat returns (move downward)
        s.Append(ctrller.redHat.transform.DOMove(ctrller.transform.position, redHatShootDuration));
        //scale the redHat from 0 to 1
        ctrller.redHat.transform.localScale=Vector3.one;
        s.Join(ctrller.redHat.transform.DOScale(Vector3.zero, redHatShootDuration));
        s.AppendCallback(()=>{
            ctrller.redHat.transform.localScale=Vector3.one;
            ctrller.redHat.SetActive(false);
            });
        //return to idle state
        s.AppendCallback(()=>animator.SetTrigger("toIdle"));
    }
}
