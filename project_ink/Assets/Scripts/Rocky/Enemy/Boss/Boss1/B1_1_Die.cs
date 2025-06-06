using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class B1_1_Die : StateBase<B1_1_Ctrller>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        DOTween.Kill(ctrller.gameObject);
        ctrller.redHat.gameObject.SetActive(false);
    }
}