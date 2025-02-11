using System.Collections;
using UnityEngine;

public class E_Monkey_attack_fly : StateBase<E_Monkey>{

const float angleRangeInRad = 30*Mathf.Deg2Rad;
    Coroutine flyCoro;
    float h;
    float phy, periodStartTime;
    float angularSpd;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.onCollisionEnter+=OnCollide;
        h=Vector2.Distance(ctrller.transform.position, PlayerShootingController.inst.transform.position);
        flyCoro=ctrller.StartCoroutine(Fly());
        phy=0;
        periodStartTime=0;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        ctrller.onCollisionEnter-=OnCollide;
        flyCoro=StopCoroutineIfNull(flyCoro);
        ctrller.rgb.velocity=Vector2.zero;
    }
    Vector2 CalculateTargetPos(float theta){
        float cos=Mathf.Cos(theta), sin=Mathf.Sin(theta);
        return new Vector2(-sin*h, cos*h)+(Vector2)PlayerShootingController.inst.transform.position;
    }
    Vector2 DirToPlayer(){
        return (ctrller.rgb.position-(Vector2)PlayerShootingController.inst.transform.position).normalized;
    }
    void DrawPoint(Vector2 point){
        Debug.DrawLine(point+new Vector2(-1,-1), point+new Vector2(1,1));
        Debug.DrawLine(point+new Vector2(-1,1), point+new Vector2(1,-1));
    }
    IEnumerator Fly(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float linearSpd=ctrller.flySpd;
        float linearSpd_deltaTime=linearSpd*Time.fixedDeltaTime;
        angularSpd=Mathf.Atan(linearSpd/h);
        float angularSpd_deltaTime=angularSpd*Time.fixedDeltaTime;
        float theta=Vector2.SignedAngle(Vector2.up, DirToPlayer())*Mathf.Deg2Rad;
        float dtheta=angularSpd_deltaTime;
        //if initially the enemy is not in (-75, 75), then fly to that range
        if(theta>angleRangeInRad)
            dtheta=-Mathf.Abs(dtheta);
        else if(theta<-angleRangeInRad)
            dtheta=Mathf.Abs(dtheta);
        
        Vector2 targetPos, vectorToTargetPos;
        float t=0;
        while(t<ctrller.flyDuration){
            //increase theta by dtheta and flip dtheta (direction) if necessary
            theta+=dtheta;
            if((theta>angleRangeInRad && dtheta>=0) || (theta<-angleRangeInRad && dtheta<=0)){
                dtheta=-dtheta;
                theta+=dtheta+dtheta;
            }

            //update target pos
            targetPos=MathUtil.GetVector_up(theta, PlayerShootingController.inst.transform.position, h);
            vectorToTargetPos=targetPos-(Vector2)ctrller.rgb.position;
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
        flyCoro=null;
    }
    void OnCollide(Collision2D collision){
        if(collision==null || GameManager.IsLayer(GameManager.inst.groundLayer,collision.gameObject.layer)){
            float x=Time.time-periodStartTime;
            phy=MathUtil.SwitchDirection(6.28318f*angularSpd, phy, x);
        }
    }
}