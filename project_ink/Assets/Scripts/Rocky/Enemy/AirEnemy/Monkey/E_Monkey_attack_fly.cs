using System.Collections;
using UnityEngine;

public class E_Monkey_attack_fly : StateBase<E_Monkey>{

    Coroutine flyCoro;
    float h;
    float flyToTime;
    float phy, periodStartTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.onCollisionEnter+=OnCollide;
        h=Vector2.Distance(ctrller.transform.position, PlayerShootingController.inst.transform.position);
        flyCoro=ctrller.StartCoroutine(Fly());
        flyToTime=ctrller.flyDuration+Time.time;
        phy=0;
        periodStartTime=0;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(Time.time>=flyToTime) animator.SetTrigger("attack");
        if(Input.GetKeyDown(KeyCode.E))
            OnCollide(null);
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
    IEnumerator Fly(){
        float epsilon=.05f;
        float theta=Vector2.SignedAngle(Vector2.up, ctrller.transform.position-PlayerShootingController.inst.transform.position)*Mathf.Deg2Rad;
        float thetaRange=Mathf.PI/6;

        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float dtheta=ctrller.flyAngluarSpd*Time.fixedDeltaTime;
        if(theta>thetaRange) dtheta=-dtheta;
        Vector2 targetPos, vectorToTargetPos;
        //move to start angle
        while(Mathf.Abs(theta-thetaRange)>epsilon){
            theta+=dtheta;
            targetPos=CalculateTargetPos(theta);
            ctrller.rgb.position=targetPos;
            yield return wait;
        }
        periodStartTime=Time.time;
        phy=0;
        while(true){
            theta=thetaRange*Mathf.Cos(6.28318f*ctrller.flyAngluarSpd*(Time.time-periodStartTime)+phy);
            targetPos=CalculateTargetPos(theta);
            vectorToTargetPos=targetPos-ctrller.rgb.position;
            float distToTarget=vectorToTargetPos.x*vectorToTargetPos.x+vectorToTargetPos.y*vectorToTargetPos.y;
            if(distToTarget>.4f){
                targetPos=(Vector2)ctrller.rgb.position+vectorToTargetPos/Mathf.Sqrt(distToTarget)*.5f;
            }
            ctrller.rgb.position=Vector2.Lerp(ctrller.rgb.position,targetPos,.3f);
            yield return wait;
        }
        flyCoro=null;
    }
    void OnCollide(Collision2D collision){
        if(collision==null || GameManager.IsLayer(GameManager.inst.groundLayer,collision.gameObject.layer)){
            float t=1/ctrller.flyAngluarSpd;
            float a=6.28318f*ctrller.flyAngluarSpd, x=Time.time-periodStartTime;
            phy-=2*((x+phy/a)%t)/t*6.28318f;
        }
    }
}