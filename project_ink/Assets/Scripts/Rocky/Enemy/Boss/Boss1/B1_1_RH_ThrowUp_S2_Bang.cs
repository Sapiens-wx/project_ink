using UnityEngine;
using DG.Tweening;

public class B1_1_RH_ThrowUp_S2_Bang : StateBase<B1_1_RedHat>
{
    /// <summary>
    /// when in normalized time of this clip should the redhat shoot
    /// </summary>
    public float shootTimeNormalized;
    public int numOfBullet;
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
            //TODO: play the BANG! effect
            //shoot bullets in a circle
            float theta=0,dtheta=Mathf.PI*2/numOfBullet;
            for(int i=0;i<numOfBullet;++i){
                //intantiate bullet
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet_dir(EnemyBulletManager.inst.boss1_s2_a5,
                    ctrller.transform.position,
                    MathUtil.Rotate(Vector2.up, theta)
                    );
                bullet.rgb.angularVelocity=ctrller.bulletAngularSpd;
                theta+=dtheta;
            }
        }
    }
}