using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GroundChase : StateBase<EnemyBase_Ground>
{
    PathNavigator pathNavigator;
    List<PathFinder.Node> paths;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(pathNavigator==null)
            pathNavigator=new PathNavigator(ctrller, ctrller.chaseSpd, ()=>ctrller.onGround);
        pathNavigator.chaseCoro = ctrller.StartCoroutine(pathNavigator.Chase(PlayerShootingController.inst.transform));
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
        ctrller.rgb.velocity=Vector2.zero;
    }
}