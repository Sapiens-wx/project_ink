using System.Collections;
using UnityEngine;

public class E1_idle : StateBase<EnemyBase_Ground>
{
    public float restInterval,restIntervalRange;
    Coroutine coro;
    Coroutine toWalkCoro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro = ctrller.StartCoroutine(m_FixedUpdate());
        toWalkCoro=ctrller.StartCoroutine(RandWalk());
        ctrller.rgb.velocity=new Vector2(0, ctrller.rgb.velocity.y);
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
        if(toWalkCoro!=null){
            ctrller.StopCoroutine(toWalkCoro);
            toWalkCoro=null;
        }
    }
    IEnumerator RandWalk(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(!ctrller.onGround){
            yield return wait;
        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(restInterval-restIntervalRange/2, restInterval+restIntervalRange/2));
        ctrller.animator.SetTrigger("patrol");
        toWalkCoro=null;
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            yield return wait;
        }
    }
}