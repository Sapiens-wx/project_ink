using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Mouse_patrol : StateBase<E_Mouse>
{
    public float patrolDist, patrolDistRange;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        float actualPatrolDist=patrolDist+Random.Range(-patrolDistRange/2, patrolDistRange/2);
        Vector3 pos=ctrller.transform.position;
        if(ctrller.Dir==1){
            pos.x+=actualPatrolDist;
            if(pos.x>ctrller.patrol_xmax){
                pos.x=ctrller.patrol_xmax-pos.x+ctrller.patrol_xmax;
                ctrller.Dir=-1;
            }
        } else{
            pos.x-=actualPatrolDist;
            if(pos.x<ctrller.patrol_xmin){
                pos.x=ctrller.patrol_xmin+ctrller.patrol_xmin-pos.x;
                ctrller.Dir=1;
            }
        }
        ctrller.transform.position=pos;
        animator.SetTrigger("idle");
    }
}