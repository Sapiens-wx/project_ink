using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_GroundChase : StateBase<EnemyBase_Ground>
{
    Coroutine chaseCoro;
    List<PathFinder.Node> paths;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
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
        chaseCoro=StopCoroutineIfNull(chaseCoro);
        ctrller.rgb.velocity=Vector2.zero;
    }
    Vector2 GetHorizontalJumpVelocity(Vector2 from, Vector2 to){
        //calculate velocity
        Vector2 v=new Vector2(ctrller.Dir==1?ctrller.chaseSpd:-ctrller.chaseSpd,0);
        float jumpTime=(to.x-from.x)/v.x;
        float gravity=ctrller.rgb.gravityScale*9.8f;
        v.y=gravity*jumpTime/2; //vy=gt/2
        v.y+=.1f; //even with exact value, the enemy still jumps lower than desired height
        return v;
    }
    Vector2 GetJumpVelocity_exact(Vector2 from, Vector2 to){
        RaycastHit2D hit = Physics2D.Raycast(to, Vector2.down, float.MaxValue, GameManager.inst.groundLayer);
        if(!hit){
            Debug.LogError("no platform to land on");
            return Vector2.zero;
        }
        //calculate the exact [to] position: just when the ctrller's corner touches the platform's corner
        Vector2 min=hit.collider.bounds.min, max=hit.collider.bounds.max;
        Vector2 bcExtents=ctrller.bc.bounds.extents, bcOffset=ctrller.bc.offset;
        if(ctrller.Dir==1){ //jump right up
            to=new Vector2(min.x-bcExtents.x+bcOffset.x, max.y+bcExtents.y+bcOffset.y);
        } else //jump left up
            to=new Vector2(max.x+bcExtents.x+bcOffset.x, max.y+bcExtents.y+bcOffset.y);
        //calculate velocity
        Vector2 v=Vector2.zero;
        float jumpTime=.5f;
        float h=to.y-from.y;
        float gravity=ctrller.rgb.gravityScale*9.8f;
        v.y=h/jumpTime+.5f*gravity*jumpTime; //v=h/t+.5*gt
        v.x=(to.x-from.x)/jumpTime+.1f;
        v.y+=.3f/Mathf.Max(v.y,1); //even with exact value, the enemy still jumps lower than desired height
        return v;
    }
    /* legacy
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
    }*/
    PathFinder.Node NearestNodeToTarget(Transform target){
        return PathFinder.inst.GetNeareastNode_g(target.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    PathFinder.Node NearestNodeToThis(){
        return PathFinder.inst.GetNeareastNode_g(ctrller.transform.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    void FindPath(Transform target){
        paths=PathFinder.inst.FindPath_g(ctrller.transform.position, target.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    bool CheckStucked(float from){
        return Time.time-from>10;
    }
    IEnumerator Chase(Transform target){
        float epsilon=0.4f; //used to check whether two points are close enough
        WaitForSeconds detectInterval=new WaitForSeconds(.05f);
        FindPath(target);
        PathFinder.Node cur=paths[0],prev;
        //move from the first node to the last
        int i=1;
        //when i<paths.Count, the enemy moves to paths[i]. when i==paths.Count,
        //the enemy moves toward the player.
        for(;i<=paths.Count;){ 
            Vector2 v;
            if(i==paths.Count){ //reaches the nearest node to the player. now moves toward the player
                ctrller.Dir=PlayerShootingController.inst.transform.position.x>ctrller.transform.position.x?1:-1;
                v=new Vector2(ctrller.Dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
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
                float moveStartTime=Time.time;
                ctrller.Dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
                if(cur.gridPos.y==prev.gridPos.y){
                    if(PathFinder.inst.NeedsJump(prev, cur)){ //horizontal jump
                        v=GetHorizontalJumpVelocity(ctrller.transform.position, cur.worldPos);
                        ctrller.rgb.velocity=v;
                        for(;Vector2.Distance(ctrller.transform.position, cur.worldPos)>epsilon;){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    } else{ //move horizontally
                        v=new Vector2(ctrller.Dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
                        ctrller.rgb.velocity=v;
                        for(;Mathf.Abs(ctrller.transform.position.x-cur.worldPos.x)>epsilon;){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    }
                }
                else if(cur.gridPos.y>prev.gridPos.y){ //jump down
                    v=new Vector2(ctrller.Dir==1?ctrller.chaseSpd:-ctrller.chaseSpd, ctrller.rgb.velocity.y);
                    ctrller.rgb.velocity=v;
                    float edgeXPos;
                    if(ctrller.Dir==1){ //edge is on the right
                        float boundsLeft=ctrller.bc.bounds.min.x;
                        edgeXPos=Mathf.Max(prev.worldPos.x+PathFinder.inst.GridSize.x/2, cur.worldPos.x-ctrller.bc.bounds.size.x/2);
                        bool wasInAir=false;
                        for(;edgeXPos>=boundsLeft || (!wasInAir || !ctrller.onGround);){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos<boundsLeft)
                                v.x=0;
                            boundsLeft=ctrller.bc.bounds.min.x;
                            if(!ctrller.onGround) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked in jump down. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    } else{
                        float boundsRight=ctrller.bc.bounds.max.x;
                        edgeXPos=Mathf.Min(prev.worldPos.x-PathFinder.inst.GridSize.x/2, cur.worldPos.x+ctrller.bc.bounds.size.x/2);
                        bool wasInAir=false;
                        for(;edgeXPos<=boundsRight || (!wasInAir || !ctrller.onGround);){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos>boundsRight)
                                v.x=0;
                            boundsRight=ctrller.bc.bounds.max.x;
                            if(!ctrller.onGround) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked in jump down. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    }
                } else{ //jump up
                    //v=GetJumpVelocity(ctrller.transform.position, cur.worldPos+
                    //    new Vector2(-ctrller.Dir*(PathFinder.inst.GridSize.x/2+ctrller.bounds.max.x),
                    //    ctrller.bounds.max.y-PathFinder.inst.GridSize.y/2));
                    v=GetJumpVelocity_exact(ctrller.transform.position, cur.worldPos);
                    ctrller.rgb.velocity=v;
                    for(;Vector2.Distance(ctrller.transform.position, cur.worldPos)>epsilon;){
                        if((ctrller.Dir==1&&ctrller.transform.position.x>cur.worldPos.x)||(ctrller.Dir==-1&&ctrller.transform.position.x<cur.worldPos.x)){
                            v.x=0;
                        }
                        if(v.x==0&&ctrller.onGround) break;
                        ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                    while(!ctrller.onGround)
                        yield return detectInterval;
                }
                }
                //detect if the target node changes
                PathFinder.Node targetNode=NearestNodeToTarget(target);
                PathFinder.Node fromNode=NearestNodeToThis();

                if(targetNode!=paths[^1] || fromNode !=paths[i-1]){
                    paths=PathFinder.inst.FindPath_g(fromNode, targetNode);
                    i=1;
                } else ++i;
        }
        ctrller.rgb.velocity=Vector2.zero;
    }
}