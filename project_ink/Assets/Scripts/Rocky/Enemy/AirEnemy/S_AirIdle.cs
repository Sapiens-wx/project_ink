using System.Collections;
using UnityEngine;

public class S_AirIdle : StateBase<EnemyBase_Air>
{
    Vector2 stayPos;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        stayPos = FindPlaceToStay();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.transform.position=stayPos;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(ctrller==null) ctrller=animator.GetComponent<EnemyBase_Air>();
        Vector3 scale = ctrller.transform.localScale;
        scale.y=Mathf.Abs(scale.y);
        ctrller.transform.localScale=scale;
    }
    Vector2 FindPlaceToStay(){
        RaycastHit2D hit=Physics2D.Raycast(ctrller.transform.position, new Vector2(0, ctrller.restDir), float.MaxValue, GameManager.inst.groundLayer);
        if(!hit){
            Debug.LogError("S_AirIdle did not find a place to stay");
            return ctrller.transform.position;
        }
        Vector2 newPos=hit.point;
        newPos.y+=ctrller.restDir==1?-ctrller.bc.bounds.extents.y:ctrller.bc.bounds.extents.y;
        ctrller.transform.position=newPos;
        Vector3 scale = ctrller.transform.localScale;
        scale.y=ctrller.restDir==1?-Mathf.Abs(scale.y):Mathf.Abs(scale.y);
        ctrller.transform.localScale=scale;
        return newPos;
    }
}