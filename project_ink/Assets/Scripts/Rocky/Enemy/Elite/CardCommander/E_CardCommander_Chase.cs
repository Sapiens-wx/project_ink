using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is copied entirely from S_GroundChase. only modified slightly.
public class E_CardCommander_Chase : StateBase<E_CardCommander>
{
    PathNavigator pathNavigator;
    Coroutine switchToAttackCoro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(pathNavigator==null)
            pathNavigator=new PathNavigator(ctrller, ctrller.chaseSpd, ()=>ctrller.onGround);
        pathNavigator.chaseCoro = ctrller.StartCoroutine(pathNavigator.Chase(PlayerShootingController.inst.transform));
        if(!animator.GetBool("b_action1")){
            switchToAttackCoro=ctrller.StartCoroutine(SwitchToAttack());
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
        pathNavigator.chaseCoro=StopCoroutineIfNull(pathNavigator.chaseCoro);
        switchToAttackCoro=StopCoroutineIfNull(switchToAttackCoro);
        ctrller.rgb.velocity=Vector2.zero;
    }
    IEnumerator SwitchToAttack(){
        yield return new WaitForSeconds(1);
        ctrller.animator.Play("attack",0);
    }
}