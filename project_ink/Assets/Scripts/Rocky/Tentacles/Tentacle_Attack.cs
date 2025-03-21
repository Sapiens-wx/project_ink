using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Attack : StateBase<Tentacle>
{
    public bool reverse;
    float startLen,endLen;
    Coroutine coro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(reverse){
            startLen=ctrller.len_attack;
            endLen=ctrller.len_idle;
        } else{
            startLen=ctrller.len_idle;
            endLen=startLen;
            coro=Tentacle.inst.StartCoroutine(Adjust());
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.len=Mathf.Lerp(startLen, endLen, stateInfo.normalizedTime);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(coro!=null){
            Tentacle.inst.StopCoroutine(coro);
            coro=null;
        }
        Tentacle.inst.len=endLen;
    }
    IEnumerator Adjust(){
        WaitForSeconds wait=new WaitForSeconds(.2f);
        while(true){
            Vector2 dir=Tentacle.inst.target-(Vector2)PlayerCtrl.inst.transform.position;
            float length=Mathf.Min(dir.magnitude,Tentacle.inst.maxLength);
            dir/=length;
            if(Tentacle.inst.target.x<PlayerCtrl.inst.transform.position.x){
                dir=MathUtil.Rotate(dir, -Mathf.PI/2);
                Tentacle.inst.Dir=1;
            } else{
                dir=MathUtil.Rotate(dir, Mathf.PI/2);
                Tentacle.inst.Dir=-1;
            }
            endLen=length;
            Tentacle.inst.len_attack=endLen;
            Tentacle.inst.anchorParent.eulerAngles=new Vector3(0,0,Vector2.SignedAngle(Vector2.up, dir));
            yield return wait;
        }
    }
}
