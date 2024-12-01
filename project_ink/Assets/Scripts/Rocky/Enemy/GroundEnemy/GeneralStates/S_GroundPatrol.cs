using System.Collections;
using UnityEngine;

public class S_GroundPatrol : StateBase<EnemyBase_Ground> {
    public float patrolSpd;
    Coroutine coro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //randomly choose a direction to begin
        ctrller.Dir=Random.Range(0,2)==0?-1:1;
        ctrller.rgb.velocity=new Vector2(ctrller.Dir==1?patrolSpd:-patrolSpd, ctrller.rgb.velocity.y);
        //fixed update
        coro=ctrller.StartCoroutine(m_FixedUpdate());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ctrller.StopCoroutine(coro);
        coro=null;
        ctrller.rgb.velocity=new Vector2(0,ctrller.rgb.velocity.y);
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            //if reach the edge, then turn and continue walking
            if(ctrller.transform.position.x>ctrller.patrol_xmax){
                Vector3 pos=ctrller.transform.position;
                pos.x=ctrller.patrol_xmax;
                ctrller.transform.position=pos;
                ctrller.Dir=-1;
                ctrller.rgb.velocity=new Vector2(-patrolSpd,ctrller.rgb.velocity.y);
            }
            else if(ctrller.transform.position.x<ctrller.patrol_xmin){
                Vector3 pos=ctrller.transform.position;
                pos.x=ctrller.patrol_xmin;
                ctrller.transform.position=pos;
                ctrller.Dir=1;
                ctrller.rgb.velocity=new Vector2(patrolSpd,ctrller.rgb.velocity.y);
            }
            yield return wait;
        }
    }
}