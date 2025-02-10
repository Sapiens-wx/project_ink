using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Carrot_Attack : StateBase<E_Carrot>
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.Dir=(int)Mathf.Sign(PlayerShootingController.inst.transform.position.x-ctrller.transform.position.x);
        ctrller.rgb.velocity=new Vector2(ctrller.Dir==1? ctrller.attackDashSpd:-ctrller.attackDashSpd, 0);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 tmp=ctrller.transform.position;
        if(ctrller.Dir==1 && ctrller.transform.position.x>ctrller.patrol_xmax){ //reach the end of the platform on the right
            ctrller.Dir=-1;
            tmp.x=ctrller.patrol_xmax;
            ctrller.transform.position=tmp;
            ctrller.rgb.velocity=new Vector2(-ctrller.attackDashSpd,0);
            if(!ctrller.inHatred)
                animator.SetTrigger("idle");
        } else if(ctrller.Dir==-1 && ctrller.transform.position.x<ctrller.patrol_xmin){ //reach the end on the left
            ctrller.Dir=1;
            tmp.x=ctrller.patrol_xmin;
            ctrller.transform.position=tmp;
            ctrller.rgb.velocity=new Vector2(ctrller.attackDashSpd,0);
            if(!ctrller.inHatred)
                animator.SetTrigger("idle");
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.rgb.velocity=Vector2.zero;
    }
}
