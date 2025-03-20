using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Attack : StateBase<Tentacle>
{
    public bool reverse;
    Vector2 target;
    float startLen,endLen;
    Coroutine coro;
    Tentacle tentacle;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        tentacle=animator.GetComponent<Tentacle>();
        if(reverse){ //attack recover
            startLen=ctrller.len_attack;
            endLen=ctrller.len_idle;
        } else{ //attack
            target=tentacle.GetNextAttack();
            startLen=ctrller.len_idle;
            endLen=startLen;
            coro=tentacle.StartCoroutine(Adjust());
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.len=Mathf.Lerp(startLen, endLen, stateInfo.normalizedTime);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(coro!=null){
            tentacle.StopCoroutine(coro);
            coro=null;
        }
        tentacle.len=endLen;
    }
    IEnumerator Adjust(){
        WaitForSeconds wait=new WaitForSeconds(.2f);
        while(true){
            Vector2 dir=target-(Vector2)PlayerCtrl.inst.transform.position;
            float length=Mathf.Min(dir.magnitude,tentacle.maxLength);
            dir/=length;
            if(target.x<PlayerCtrl.inst.transform.position.x){
                dir=MathUtil.Rotate(dir, -Mathf.PI/2);
                tentacle.Dir=1;
            } else{
                dir=MathUtil.Rotate(dir, Mathf.PI/2);
                tentacle.Dir=-1;
            }
            endLen=length;
            tentacle.len_attack=endLen;
            tentacle.anchorParent.eulerAngles=new Vector3(0,0,Vector2.SignedAngle(Vector2.up, dir));
            yield return wait;
        }
    }
}
