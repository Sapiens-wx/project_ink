using System.Collections;
using UnityEngine;

public class E_Monkey_attack_fly : StateBase<E_Monkey>{

    Coroutine flyCoro;
    float h;
    float flyToTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        h=Vector2.Distance(ctrller.transform.position, PlayerShootingController.inst.transform.position);
        flyCoro=ctrller.StartCoroutine(Fly());
        flyToTime=ctrller.flyDuration+Time.time;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(Time.time>=flyToTime) animator.SetTrigger("attack");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        flyCoro=StopCoroutineIfNull(flyCoro);
        ctrller.rgb.velocity=Vector2.zero;
    }
    Vector2 CalculateTargetPos(float theta){
        float cos=Mathf.Cos(theta), sin=Mathf.Sin(theta);
        return new Vector2(-sin*h, cos*h)+(Vector2)PlayerShootingController.inst.transform.position;
    }
    IEnumerator Fly(){
        float epsilon=.05f;
        float theta=Vector2.SignedAngle(Vector2.up, ctrller.transform.position-PlayerShootingController.inst.transform.position)*Mathf.Deg2Rad;
        float thetaRange=Mathf.PI/6;

        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float dtheta=ctrller.flyAngluarSpd*Time.fixedDeltaTime;
        if(theta>thetaRange) dtheta=-dtheta;
        Vector2 targetPos;
        //move to start angle
        while(Mathf.Abs(theta-thetaRange)>epsilon){
            theta+=dtheta;
            targetPos=CalculateTargetPos(theta);
            ctrller.transform.position=targetPos;
            yield return wait;
        }
        float periodStartTime=Time.time;
        while(true){
            theta=thetaRange*Mathf.Cos(6.28318f*ctrller.flyAngluarSpd*(Time.time-periodStartTime));
            targetPos=CalculateTargetPos(theta);
            ctrller.transform.position=targetPos;
            yield return wait;
        }
        flyCoro=null;
    }
}