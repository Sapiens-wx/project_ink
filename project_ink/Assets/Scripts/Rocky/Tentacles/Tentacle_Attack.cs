using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Attack : StateBase<Tentacle>
{
    public bool reverse;
    Vector2 target;
    float startLen,endLen;
    Coroutine coro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(reverse){ //attack recover
            startLen=ctrller.len;
            endLen=ctrller.len_idle;
        } else{ //attack
            target=ctrller.GetNextAttack();
            startLen=ctrller.len_idle;
            endLen=startLen;
            coro=ctrller.StartCoroutine(Adjust());
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.len=Mathf.Lerp(startLen, endLen, stateInfo.normalizedTime);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(coro!=null){
            ctrller.StopCoroutine(coro);
            coro=null;
        }
        ctrller.len=endLen;
        if(reverse)
            ctrller.onAttackEnd?.Invoke(ctrller);
    }
    IEnumerator Adjust(){
        WaitForSeconds wait=new WaitForSeconds(.2f);
        while(true){
            Vector2 dir=target-(Vector2)ctrller.transform.position;
            float length=Mathf.Clamp(dir.magnitude, ctrller.minLength, ctrller.maxLength);
            dir/=length;
            if(target.x<ctrller.transform.position.x){
                dir=MathUtil.Rotate(dir, -Mathf.PI/2);
            } else{
                dir=MathUtil.Rotate(dir, Mathf.PI/2);
            }
            endLen=length;
            ctrller.anchorParent.eulerAngles=new Vector3(0,0,Vector2.SignedAngle(Vector2.up, dir));
            yield return wait;
        }
    }
}
