using System.Collections;
using UnityEngine;

public class S_GroundPatrol2 : StateBase<EnemyBase_Ground>
{
    public float patrolInterval,patrolIntervalRange;
    float patrolUntilTime;
    Coroutine coro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro = ctrller.StartCoroutine(m_FixedUpdate());
        ctrller.rgb.velocity=new Vector2(ctrller.Dir==1?ctrller.walkSpd:-ctrller.walkSpd, ctrller.rgb.velocity.y);
        patrolUntilTime=Time.time+Random.Range(patrolInterval-patrolIntervalRange/2, patrolInterval+patrolIntervalRange/2);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.StopCoroutine(coro);
        coro=null;
        ctrller.rgb.velocity=new Vector2(0, ctrller.rgb.velocity.y);
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            //if reach walk time, then stop
            if(Time.time>=patrolUntilTime){
                ctrller.animator.SetTrigger("idle");
            }
            //if reach the edge, then turn and continue patroling
            if(ctrller.transform.position.x>ctrller.patrol_xmax){
                Vector3 pos=ctrller.transform.position;
                pos.x=ctrller.patrol_xmax;
                ctrller.transform.position=pos;
                ctrller.Dir=-1;
                ctrller.rgb.velocity=new Vector2(-ctrller.walkSpd, ctrller.rgb.velocity.y);
            }
            else if(ctrller.transform.position.x<ctrller.patrol_xmin){
                Vector3 pos=ctrller.transform.position;
                pos.x=ctrller.patrol_xmin;
                ctrller.transform.position=pos;
                ctrller.Dir=1;
                ctrller.rgb.velocity=new Vector2(ctrller.walkSpd, ctrller.rgb.velocity.y);
            }
            yield return wait;
        }
    }
}