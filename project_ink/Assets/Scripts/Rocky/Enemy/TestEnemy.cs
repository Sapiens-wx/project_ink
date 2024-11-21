using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public Bounds bounds;
    public Vector2 endPos;
    public float xspd;
    public float detectDist;
    public Transform target;

    Rigidbody2D rgb;
    Collider2D bc;
    List<VerticalPathFinder.Node> paths;
    //ground detection
    Vector2 groundDetectLB, groundDetectRB;
    bool onGround, prevOnGround;
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(endPos, .2f);
        Gizmos.DrawWireCube(bounds.center+transform.position,bounds.size);
        //if(bc==null) bc=GetComponent<Collider2D>();
        //groundDetectLB=(Vector2)bc.bounds.min-(Vector2)transform.position;
        //groundDetectLB.y-=.01f;
        //Gizmos.DrawWireSphere(groundDetectLB+(Vector2)transform.position,.5f);
    }
    void Start()
    {
        rgb=GetComponent<Rigidbody2D>();
        bc=GetComponent<Collider2D>();
        groundDetectLB=(Vector2)bc.bounds.min-(Vector2)transform.position;
        groundDetectLB.y-=.02f;
        groundDetectRB=groundDetectLB;
        groundDetectLB.x+=0.1f*bc.bounds.size.x;
        groundDetectRB.x+=0.9f*bc.bounds.size.x;

        StartCoroutine(Chase(target));
    }
    void FixedUpdate(){
        prevOnGround=onGround;
        onGround=Physics2D.OverlapArea((Vector2)transform.position+groundDetectLB, (Vector2)transform.position+groundDetectRB, GameManager.inst.groundLayer);
    }
    Vector2 GetJumpVelocity(Vector2 from, Vector2 to){
        Vector2 v=Vector2.zero;
        v.x=from.x<to.x?xspd:-xspd;
        float gravity=rgb.gravityScale*9.8f;
        float vx2=v.x*v.x;
        float h=to.y-from.y;
        float s=Mathf.Abs(to.x-from.x);
        v.y=h*xspd/s+s*gravity/(2*xspd);
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
        return VerticalPathFinder.inst.GetNeareastNode(target.position, bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    VerticalPathFinder.Node NearestNodeToThis(){
        return VerticalPathFinder.inst.GetNeareastNode(transform.position, bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    void FindPath(Transform target){
        paths=VerticalPathFinder.inst.FindPath(transform.position, target.position, bounds.min.y+VerticalPathFinder.inst.GridSize.y/2);
    }
    bool CheckStucked(float from){
        return Time.time-from>10;
    }
    IEnumerator Move(){
        WaitForSeconds detectInterval=new WaitForSeconds(.2f);
        VerticalPathFinder.Node cur=paths[0],prev;
        //move to the first node
        /*rgb.velocity=new Vector2((cur.worldPos.x>transform.position.x)?xspd:-xspd, rgb.velocity.y);
        for(;Vector2.Distance(transform.position, cur.worldPos)>detectDist;)
            yield return detectInterval;*/
        //move from the first node to the last
        for(int i=1;i<paths.Count;++i){
            cur=paths[i];
            prev=paths[i-1];
            int dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
            Vector2 v;
            target.transform.position=cur.worldPos;
            if(cur.gridPos.y==prev.gridPos.y){ //move horizontally
                v=new Vector2(dir==1?xspd:-xspd, rgb.velocity.y);
                rgb.velocity=v;
                for(;Mathf.Abs(transform.position.x-cur.worldPos.x)>detectDist;){
                    rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                    yield return detectInterval;
                }
            } else if(cur.gridPos.y>prev.gridPos.y){ //jump down
                v=new Vector2(dir==1?xspd:-xspd, rgb.velocity.y);
                rgb.velocity=v;
                float edgeXPos;
                if(dir==1){ //edge is on the right
                    edgeXPos=prev.worldPos.x+VerticalPathFinder.inst.GridSize.x/2;
                    for(;edgeXPos>=transform.position.x+bounds.min.x;){
                        rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                        yield return detectInterval;
                    }
                } else{
                    edgeXPos=prev.worldPos.x-VerticalPathFinder.inst.GridSize.x/2;
                    for(;edgeXPos<=transform.position.x-bounds.min.x;){
                        rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                        yield return detectInterval;
                    }
                }
            }
            else{ //jump up
                v=GetJumpVelocity(transform.position, cur.worldPos+
                    new Vector2(-dir*(VerticalPathFinder.inst.GridSize.x/2),
                    bounds.extents.y-bounds.center.y-VerticalPathFinder.inst.GridSize.y/2));
                rgb.velocity=v;
                for(;Vector2.Distance(transform.position, cur.worldPos)>detectDist;){
                    Debug.Log("update v " + v.x);
                    rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                    yield return detectInterval;
                }
            }
        }
        rgb.velocity=Vector2.zero;
    }
    IEnumerator Chase(Transform target){
        WaitForSeconds detectInterval=new WaitForSeconds(.05f);
        FindPath(target);
        VerticalPathFinder.Node cur=paths[0],prev;
        //move from the first node to the last
        int i=1;
        for(;i<paths.Count;){
            cur=paths[i];
            prev=paths[i-1];
            float moveStartTime=Time.time;
            int dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
            Vector2 v;
            if(cur.gridPos.y==prev.gridPos.y){ //move horizontally
                v=new Vector2(dir==1?xspd:-xspd, rgb.velocity.y);
                rgb.velocity=v;
                for(;Mathf.Abs(transform.position.x-cur.worldPos.x)>detectDist;){
                    rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                    if(CheckStucked(moveStartTime)){
                        Debug.LogWarning("the enemy might be stucked. restart path finding");
                        break;
                    }
                    yield return detectInterval;
                }
            } else if(cur.gridPos.y>prev.gridPos.y){ //jump down
                v=new Vector2(dir==1?xspd:-xspd, rgb.velocity.y);
                rgb.velocity=v;
                float edgeXPos;
                if(dir==1){ //edge is on the right
                    float boundsLeft=bounds.min.x;
                    edgeXPos=Mathf.Max(prev.worldPos.x+VerticalPathFinder.inst.GridSize.x/2, cur.worldPos.x+VerticalPathFinder.inst.GridSize.x/2-bounds.size.x*1.5f);
                    bool wasInAir=false;
                    for(;edgeXPos>=transform.position.x+boundsLeft || (!wasInAir || !onGround);){
                        rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                        if(edgeXPos<transform.position.x+boundsLeft){
                            v.x=0;
                        }
                        if(!onGround) wasInAir=true;
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                } else{
                    float boundsRight=bounds.max.x;
                    edgeXPos=Mathf.Min(prev.worldPos.x+VerticalPathFinder.inst.GridSize.x/2, cur.worldPos.x-VerticalPathFinder.inst.GridSize.x/2+bounds.size.x*1.5f);
                    bool wasInAir=false;
                    for(;edgeXPos<=transform.position.x+boundsRight || (!wasInAir || !onGround);){
                        rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                        if(edgeXPos>transform.position.x+boundsRight)
                            v.x=0;
                        if(!onGround) wasInAir=true;
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                }
            }
            else{ //jump up
                v=GetJumpVelocity(transform.position, cur.worldPos+
                    new Vector2(-dir*(VerticalPathFinder.inst.GridSize.x/2+bounds.max.x),
                    bounds.max.y-VerticalPathFinder.inst.GridSize.y/2));
                rgb.velocity=v;
                for(;Vector2.Distance(transform.position, cur.worldPos)>detectDist;){
                    rgb.velocity=new Vector2(v.x, rgb.velocity.y);
                    if(CheckStucked(moveStartTime)){
                        Debug.LogWarning("the enemy might be stucked. restart path finding");
                        break;
                    }
                    yield return detectInterval;
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
        rgb.velocity=Vector2.zero;
    }

}
