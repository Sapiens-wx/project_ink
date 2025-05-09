using UnityEngine;
using System.Collections;

public class E_Elf_fly : StateBase<E_Elf>{
    public float spdMin, spdMax;

    const float angleRangeInDegree=75f;
    const float angleRangeInRad=75*Mathf.Deg2Rad;

    Coroutine flyCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        flyCoro=ctrller.StartCoroutine(Fly());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        flyCoro=StopCoroutineIfNull(flyCoro);
        ctrller.rgb.velocity=Vector2.zero;
    }
    Vector2 DirToPlayer(){
        return (ctrller.rgb.position-(Vector2)PlayerShootingController.inst.transform.position).normalized;
    }
    IEnumerator Fly(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float linearSpd_deltaTime=Time.fixedDeltaTime*UnityEngine.Random.Range(spdMin, spdMax);
        float angularSpd=Mathf.Atan(linearSpd_deltaTime/ctrller.attackTriggerDist);
        float theta=Vector2.SignedAngle(Vector2.up, DirToPlayer())*Mathf.Deg2Rad;
        float dtheta=angularSpd;
        //if initially the enemy is not in (-75, 75), then fly to that range
        float initialAngle=theta*Mathf.Rad2Deg;
        if(initialAngle>angleRangeInDegree)
            dtheta=-Mathf.Abs(dtheta);
        else if(initialAngle<-angleRangeInDegree)
            dtheta=Mathf.Abs(dtheta);
        else
            dtheta=Random.Range(0,2)==0?dtheta:-dtheta;
        
        Vector2 targetPos, vectorToTargetPos;
        float t=0;
        while(t<ctrller.flyInterval){
            //increase theta by dtheta and flip dtheta (direction) if necessary
            theta+=dtheta;
            if((theta>angleRangeInRad && dtheta>=0) || (theta<-angleRangeInRad && dtheta<=0)){
                dtheta=-dtheta;
                theta+=dtheta+dtheta;
            }

            //update target pos
            targetPos=MathUtil.GetVector_up(theta, PlayerShootingController.inst.transform.position, ctrller.attackTriggerDist);
            vectorToTargetPos=targetPos-ctrller.rgb.position;
            float distToTarget=vectorToTargetPos.magnitude;
            if(distToTarget>linearSpd_deltaTime){
                targetPos=(Vector2)ctrller.rgb.position+vectorToTargetPos/distToTarget*linearSpd_deltaTime;
            }
            ctrller.rgb.position=targetPos;

            //increase time
            t+=Time.fixedDeltaTime;
            yield return wait;
        }
        ctrller.animator.SetTrigger("attack");
    }
}