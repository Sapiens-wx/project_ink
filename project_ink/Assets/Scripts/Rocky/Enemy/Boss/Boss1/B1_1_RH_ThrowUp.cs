using UnityEngine;
using DG.Tweening;

public class B1_1_RH_ThrowUp : StateBase<B1_1_RedHat>
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
            float rangexmin=RoomManager.CurrentRoom.RoomBounds.min.x, rangexmax=RoomManager.inst.RoomBounds.max.x;
            float rangexInterval=(rangexmax-rangexmin)/3;
            for(int i=0;i<3;++i){
                //intantiate bullet
                EnemyBulletBase bullet=EnemyBulletManager.InstantiateBullet(EnemyBulletManager.inst.boss1_a5);
                bullet.rgb.angularVelocity=ctrller.bulletAngularSpd;
                bullet.transform.position=ctrller.transform.position;
                //x position of the bullet when hit the ground
                float g=bullet.rgb.gravityScale*9.8f, y1=RoomManager.inst.RoomBounds.min.y;
                float vy=Mathf.Sqrt(2*g*(RoomManager.CurrentRoom.RoomBounds.max.y-ctrller.transform.position.y)); //vy=2g*height
                float randBulletX=Random.Range(rangexmin, rangexmin+rangexInterval);
                float distx=randBulletX-bullet.transform.position.x;
                float t1=vy/g;
                float h0=vy*t1-0.5f*g*t1*t1;
                float h=bullet.transform.position.y+h0-y1;
                float t2=Mathf.Sqrt(2*h/g);
                float t=t1+t2;
                float vx=distx/t;
                bullet.rgb.velocity=new Vector2(vx,vy);
                rangexmin+=rangexInterval;
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ctrller.boss.animator.SetTrigger("toIdle");
        //tell the boss to finish this action
        Sequence s=DOTween.Sequence();
        //redhat returns (move downward)
        s.Append(ctrller.transform.DOMove(ctrller.boss.StomachGlobalPos, ctrller.boss.a4_redHatShootDuration));
        //scale the redHat from 0 to 1
        ctrller.transform.localScale=Vector3.one;
        s.Join(ctrller.transform.DOScale(Vector3.zero, ctrller.boss.a4_redHatShootDuration));
        s.AppendCallback(()=>{
            ctrller.gameObject.SetActive(false);
            ctrller.transform.localScale=Vector3.one;
            });
    }
}