using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GroundChase : StateBase<EnemyBase_Ground>
{
    Coroutine coro, chaseCoro;
    List<VerticalPathFinder.Node> paths;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        coro = ctrller.StartCoroutine(m_FixedUpdate());
        chaseCoro = ctrller.StartCoroutine(Chase(PlayerShootingController.inst.transform));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.StopCoroutine(coro);
        coro=null;
    }
    IEnumerator m_FixedUpdate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        while(true){
            yield return wait;
        }
    }
    Vector2 GetJumpVelocity(Vector2 from, Vector2 to){
        Vector2 v=Vector2.zero;
        v.x=from.x<to.x?ctrller.chaseSpd:-ctrller.chaseSpd;
        float gravity=ctrller.rgb.gravityScale*9.8f;
        float vx2=v.x*v.x;
        float h=to.y-from.y;
        float s=Mathf.Abs(to.x-from.x);
        v.y=h*ctrller.chaseSpd/s+s*gravity/(2*ctrller.chaseSpd);
        //if it jumps too high, lower the vx
        if(Mathf.Abs(from.y-to.y)>=.5f && v.y*v.y/(2*gravity)>1.5f*h){
            v.y=Mathf.Sqrt(2*gravity*h);
            float originalVY=v.y;
            float two_vy_x=2*v.y*s;
            float sqrt=4*v.y*v.y*s*s-8*h*gravity*s*s;
            if(sqrt<0){
                v.y=originalVY;
            } else{
                sqrt=Mathf.Sqrt(sqrt);
                if(two_vy_x-sqrt<0)
                    v.x=(two_vy_x+sqrt)/(4*h);
                else
                    v.x=(two_vy_x-sqrt)/(4*h);
                if(from.x>to.x) v.x=-v.x;
            }
        }
        return v;
    }
    VerticalPathFinder.Node NearestNodeToTarget(Transform target){
        return VerticalPathFinder.inst.GetNeareastNode(target.position, ctrller.bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    VerticalPathFinder.Node NearestNodeToThis(){
        return VerticalPathFinder.inst.GetNeareastNode(ctrller.transform.position, ctrller.bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    void FindPath(Transform target){
        paths=VerticalPathFinder.inst.FindPath(ctrller.transform.position, target.position, ctrller.bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    bool CheckStucked(float from){
        return Time.time-from>10;
    }
    IEnumerator Chase(Transform target){
        float epsilon=0.2f; //used to check whether two points are close enough
        WaitForSeconds detectInterval=new WaitForSeconds(.05f);
        FindPath(target);
        VerticalPathFinder.Node cur=paths[0],prev;
        //move from the first node to the last
        int i=1;
        //when i<paths.Count, the enemy moves to paths[i]. when i==paths.Count,
        //the enemy moves toward the player.
        for(;i<=paths.Count;){ 
            Vector2 v;
            if(i==paths.Count){ //reaches the nearest node to the player. now moves toward the player
                int dir=PlayerShootingController.inst.transform.position.x>ctrller.transform.position.x?1:-1;
                v=new Vector2(dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
                //moves directly toward the player.
                //this loop is infinite because once the player is in the attack range,
                //the "b_attack" bool will be set true by MobBase.
                for(;;){
                    ctrller.rgb.velocity=v;
                    yield return detectInterval;
                }
            } else{ //i<paths.Count. Move through even nodes in [paths]
                cur=paths[i];
                prev=paths[i-1];
                Debug.Log($"going to node {cur.gridPos}, {cur.worldPos}");
                float moveStartTime=Time.time;
                int dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
                if(cur.gridPos.y==prev.gridPos.y){ //move horizontally
                    v=new Vector2(dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
                    ctrller.rgb.velocity=v;
                    for(;Mathf.Abs(ctrller.transform.position.x-cur.worldPos.x)>epsilon;){
                        ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                } else if(cur.gridPos.y>prev.gridPos.y){ //jump down
                    v=new Vector2(dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
                    ctrller.rgb.velocity=v;
                    float edgeXPos;
                    if(dir==1){ //edge is on the right
                        float boundsLeft=ctrller.bounds.min.x;
                        edgeXPos=Mathf.Max(prev.worldPos.x+VerticalPathFinder.inst.GridSize.x/2, cur.worldPos.x+VerticalPathFinder.inst.GridSize.x/2-ctrller.bounds.size.x*1.5f);
                        bool wasInAir=false;
                        for(;edgeXPos>=ctrller.transform.position.x+boundsLeft || (!wasInAir || !ctrller.onGround);){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos<ctrller.transform.position.x+boundsLeft){
                                v.x=0;
                            }
                            if(!ctrller.onGround) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    } else{
                        float boundsRight=ctrller.bounds.max.x;
                        edgeXPos=Mathf.Min(prev.worldPos.x+VerticalPathFinder.inst.GridSize.x/2, cur.worldPos.x-VerticalPathFinder.inst.GridSize.x/2+ctrller.bounds.size.x*1.5f);
                        bool wasInAir=false;
                        for(;edgeXPos<=ctrller.transform.position.x+boundsRight || (!wasInAir || !ctrller.onGround);){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos>ctrller.transform.position.x+boundsRight)
                                v.x=0;
                            if(!ctrller.onGround) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    }
                }
                else{ //jump up
                    v=GetJumpVelocity(ctrller.transform.position, cur.worldPos+
                        new Vector2(-dir*(VerticalPathFinder.inst.GridSize.x/2+ctrller.bounds.max.x),
                        ctrller.bounds.max.y-VerticalPathFinder.inst.GridSize.y/2));
                    ctrller.rgb.velocity=v;
                    for(;Vector2.Distance(ctrller.transform.position, cur.worldPos)>epsilon;){
                        ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                }
                }
                //detect if the target node changes
                VerticalPathFinder.Node targetNode=NearestNodeToTarget(target);
                VerticalPathFinder.Node fromNode=NearestNodeToThis();

                if(targetNode!=paths[^1] || fromNode !=paths[i-1]){
                    paths=VerticalPathFinder.inst.FindPath(fromNode, targetNode);
                    i=1;
                } else ++i;
        }
        ctrller.rgb.velocity=Vector2.zero;
    }
}