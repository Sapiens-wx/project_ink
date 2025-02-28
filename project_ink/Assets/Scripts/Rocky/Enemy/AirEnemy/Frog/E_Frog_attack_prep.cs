using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class E_Frog_attack_prep : StateBase<E_Frog>{
    Coroutine prepCoro;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        prepCoro=ctrller.StartCoroutine(ToRandomPos());
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        prepCoro=StopCoroutineIfNull(prepCoro);
    }
    PathFinder.Node NearestNodeToTarget(Vector2 pos){
        return PathFinder.inst.GetNeareastNode_a(pos);
    }
    PathFinder.Node NearestNodeToThis(){
        return PathFinder.inst.GetNeareastNode_a(ctrller.transform.position);
    }
    List<PathFinder.Node> FindPath(Vector2 pos){
        return PathFinder.inst.FindPath_a(ctrller.transform.position, pos);
    }
    IEnumerator ToRandomPos(){
        Transform target=PlayerShootingController.inst.transform;
        //select a random position
        Vector2 offset=ctrller.attackTriggerDist*Vector2.up;
        switch(Random.Range(0,3)){
            case 1: //left
                offset=MathUtil.Rotate(offset, 1.0471976f);
                break;
            case 2:
                offset=MathUtil.Rotate(offset, -1.0471976f);
                break;
        }

        //move the enemy to the position
        float epsilon=0.1f; //used to check whether two points are close enough
        WaitForSeconds detectInterval=new WaitForSeconds(.03f);
        List<PathFinder.Node> paths=FindPath((Vector2)target.position+offset);
        PathFinder.Node cur=paths[0],prev;
        //move from the first node to the last
        int i=1;
        //when i<paths.Count, the enemy moves to paths[i]. when i==paths.Count,
        //the enemy moves toward the player.
        for(;i<paths.Count;){ 
            Vector2 v;
            cur=paths[i];
            prev=paths[i-1];

            float moveStartTime=Time.time;
            ctrller.Dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
            v=(cur.worldPos-(Vector2)ctrller.transform.position).normalized*ctrller.chaseSpd;
            float dist;
            do{
                ctrller.rgb.velocity=v;
                yield return detectInterval;
                if(ctrller.rgb.velocity==Vector2.zero) break; //the frog is stucked
                Vector2 distv=(Vector2)ctrller.transform.position-cur.worldPos;
                dist=distv.x*distv.x+distv.y*distv.y;
            }while(dist>epsilon);
            //detect if the target node changes
            PathFinder.Node targetNode=NearestNodeToTarget((Vector2)target.position+offset);
            PathFinder.Node fromNode=NearestNodeToThis();

            if(targetNode!=paths[^1] || fromNode !=paths[i-1]){
                paths=FindPath((Vector2)target.position+offset);
                i=1;
            } else ++i;
        }
        ctrller.rgb.velocity=Vector2.zero;
        prepCoro=null;
        ctrller.animator.SetTrigger("attack");
    }
}