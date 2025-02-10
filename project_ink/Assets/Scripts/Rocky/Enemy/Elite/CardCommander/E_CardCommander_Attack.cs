using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CardCommander_Attack : StateBase<E_CardCommander>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        //random action
        switch(UnityEngine.Random.Range(0,2)){
            case 0: //action 1
                ctrller.StartCoroutine(Action1());
                break;
            case 1: //action 2
                if(animator.GetBool("b_action1")){
                    animator.SetBool("b_action1", false);
                    goto case 0;
                }
                goto case 0;
                ctrller.StartCoroutine(Action2());
                break;
        }
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
    IEnumerator Action1(){
        if(!ctrller.playerInAttack){
            ctrller.animator.SetBool("b_action1", true);
            ctrller.animator.SetTrigger("chase");
            yield break;
        }
        //charge
        yield return new WaitForSeconds(ctrller.chargeInterval);
        //calculate jump velocity
        float g=ctrller.rgb.gravityScale*9.8f;
        float t=Mathf.Sqrt(2*ctrller.jumpYDist/g);
        ctrller.rgb.velocity=new Vector2(ctrller.Dir*ctrller.jumpXDist/t, g*t);
        //wait until it jumps to the desired height
        yield return new WaitForSeconds(t);
        //air charge
        ctrller.rgb.velocity=Vector2.zero;
        ctrller.rgb.gravityScale=0;
        yield return new WaitForSeconds(ctrller.airChargeInterval);
        //dash
        Vector2 dir=(Vector2)PlayerShootingController.inst.transform.position-(Vector2)ctrller.transform.position;
        float dist=dir.magnitude;
        dir/=dist;
        ctrller.rgb.velocity=dir*ctrller.dashSpd;
        yield return new WaitForSeconds(dist/ctrller.dashSpd);
        ctrller.rgb.gravityScale=g/9.8f;
        ctrller.animator.SetTrigger("chase");
    }
    //Summon normal enemies
    IEnumerator Action2(){
        yield break;
    }
}
