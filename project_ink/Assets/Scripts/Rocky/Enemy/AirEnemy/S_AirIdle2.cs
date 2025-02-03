using System.Collections;
using UnityEngine;

public class S_AirIdle2 : StateBase<EnemyBase_Air>
{
    Vector2 center;
    Coroutine flyCoro;
    float phy, periodStartTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.onCollisionEnter+=OnCollide;
        center=ctrller.transform.position;
        phy=0;
        periodStartTime=0;
        flyCoro=ctrller.StartCoroutine(Fly());
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        flyCoro=StopCoroutineIfNull(flyCoro);
    }
    Vector2 CalculateTargetPos(float theta){
        float cos=Mathf.Cos(theta), sin=Mathf.Sin(theta);
        return new Vector2(-sin*ctrller.idle2_radius, cos*ctrller.idle2_radius)+center;
    }
    IEnumerator Fly(){
        float theta=0;
        float thetaRange=Mathf.PI/3;

        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        Vector2 targetPos;
        periodStartTime=Time.time;
        phy=0;
        while(true){
            theta=thetaRange*Mathf.Cos(6.28318f*ctrller.idle2_flyAngularSpd*(Time.time-periodStartTime)+phy);
            targetPos=CalculateTargetPos(theta);
            targetPos=MathUtil.ClampFromToVector(ctrller.rgb.position, targetPos, .3f);
            ctrller.rgb.position=Vector2.Lerp(ctrller.rgb.position,targetPos,.3f);
            yield return wait;
        }
    }
    void OnCollide(Collision2D collision){
        if(collision==null || GameManager.IsLayer(GameManager.inst.groundLayer,collision.gameObject.layer)){
            phy=MathUtil.SwitchDirection(6.28318f*ctrller.idle2_flyAngularSpd, phy, Time.time-periodStartTime);
        }
    }
}