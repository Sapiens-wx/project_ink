using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AirChase : StateBase<EnemyBase_Air>
{
    Coroutine chaseCoro;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        chaseCoro = ctrller.StartCoroutine(Chase(PlayerShootingController.inst.transform));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chaseCoro=StopCoroutineIfNull(chaseCoro);
    }
    PathFinder.Node NearestNodeToTarget(Transform target){
        return PathFinder.inst.GetNeareastNode_a(target.position);
    }
    PathFinder.Node NearestNodeToThis(){
        return PathFinder.inst.GetNeareastNode_a(ctrller.transform.position);
    }
    List<PathFinder.Node> FindPath(Transform target){
        return PathFinder.inst.FindPath_a(ctrller.transform.position, target.position);
    }
    IEnumerator Chase(Transform target){
        float epsilon=0.4f; //used to check whether two points are close enough
        WaitForSeconds detectInterval=new WaitForSeconds(.05f);
        List<PathFinder.Node> paths=FindPath(target);
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
                Vector2 distv=(Vector2)ctrller.transform.position-cur.worldPos;
                dist=distv.x*distv.x+distv.y*distv.y;
            }while(dist>epsilon);
            //detect if the target node changes
            PathFinder.Node targetNode=NearestNodeToTarget(target);
            PathFinder.Node fromNode=NearestNodeToThis();

            if(targetNode!=paths[^1] || fromNode !=paths[i-1]){
                paths=FindPath(target);
                i=1;
            } else ++i;
        }
        ctrller.rgb.velocity=Vector2.zero;
    }
}