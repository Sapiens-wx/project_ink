using UnityEngine;
using DG.Tweening;

public class B1_1_RH_Throw_S2 : StateBase<B1_1_RedHat>
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
        int counter=ctrller.animator.GetInteger("to_throw_S2")-1;
        ctrller.animator.SetInteger("to_throw_S2", counter);
        //tell the boss to finish this action
        if(counter<=0){
            ctrller.boss.animator.SetTrigger("toIdle");
        }
    }
}
