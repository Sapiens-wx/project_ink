using UnityEngine;
using DG.Tweening;

public class B1_1_RH_Throw : StateBase<B1_1_RedHat>
{
    /// <summary>
    /// when in normalized time of this clip should the redhat shoot
    /// </summary>
    public float shootTimeNormalized;
    bool shoot;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        shoot=false;
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.UpdateDir();
        //shoot on this frame
        if(shoot==false && stateInfo.normalizedTime>=shootTimeNormalized){
            shoot=true;
            Vector2 dir=((Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.transform.position).normalized;
            EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_a4, ctrller.transform.position, dir);
            bullet.rgb.angularVelocity=ctrller.bulletAngularSpd;
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        int counter=ctrller.animator.GetInteger("to_throw")-1;
        ctrller.animator.SetInteger("to_throw", counter);
        //tell the boss to finish this action
        if(counter<=0){
            Sequence s=DOTween.Sequence();
            //redhat returns (move downward)
            s.Append(ctrller.transform.DOMove(ctrller.boss.transform.position, ctrller.boss.a4_redHatShootDuration));
            //scale the redHat from 0 to 1
            ctrller.transform.localScale=Vector3.one;
            s.Join(ctrller.transform.DOScale(Vector3.zero, ctrller.boss.a4_redHatShootDuration));
            s.AppendCallback(()=>{
                ctrller.gameObject.SetActive(false);
                ctrller.transform.localScale=Vector3.one;
                ctrller.boss.animator.SetTrigger("toIdle");
                });
        }
    }
}
