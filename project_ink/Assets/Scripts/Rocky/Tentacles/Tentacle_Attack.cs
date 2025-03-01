using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Attack : StateBase<Tentacle>
{
    public bool reverse;
    float startLen,endLen;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(reverse){
            startLen=ctrller.len_attack;
            endLen=ctrller.len_idle;
        } else{
            startLen=ctrller.len_idle;
            endLen=ctrller.len_attack;
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.len=Mathf.Lerp(startLen, endLen, stateInfo.normalizedTime);
    }
}
