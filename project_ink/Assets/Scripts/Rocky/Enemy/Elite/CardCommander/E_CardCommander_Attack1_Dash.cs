using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CardCommander_Attack1_Dash : StateBase<E_CardCommander>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.StartCoroutine(Dash());
        ctrller.rgb.gravityScale=0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    IEnumerator Dash(){
        ++ctrller.ac1_dash_count;
        if(ctrller.ac1_dash_count==2) yield return new WaitForSeconds(ctrller.chargeInterval);
        else if(ctrller.ac1_dash_count==3){
            yield return new WaitForSeconds(ctrller.chargeInterval2);
            ctrller.ac1_dash_count=0;
        }
        //dash
        Vector2 dir=(Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.transform.position;
        float dist=dir.magnitude;
        dir/=dist;
        ctrller.rgb.velocity=dir*ctrller.dashSpd;
        yield return new WaitForSeconds(ctrller.dashDist/ctrller.dashSpd);
        ctrller.rgb.gravityScale=ctrller.gravityScale;
        ctrller.rgb.velocity=Vector2.zero;
        ctrller.animator.SetTrigger("idle");
    }
}
