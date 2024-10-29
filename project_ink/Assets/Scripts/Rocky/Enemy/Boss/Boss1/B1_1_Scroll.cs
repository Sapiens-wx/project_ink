using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_1_Scroll : StateBase<B1_1_Ctrller>
{
    [SerializeField] float frq;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        ctrller.StartCoroutine(Scroll(animator));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    IEnumerator Scroll(Animator animator){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        float timeEllapsed=0;

        //calculate parameters needed for moving
        float minX=RoomManager.CurrentRoom.RoomBounds.min.x,
            maxX=RoomManager.CurrentRoom.RoomBounds.max.x;
        float w=6.283f*frq;
        float yoffset=(maxX+minX)/2;
        float amp=(maxX-minX)/2;
        float phase=Mathf.Asin(animator.transform.position.x/amp);

        Vector3 pos=animator.transform.position;
        //scroll lasts for 5 seconds
        while(timeEllapsed<5){
            pos.x=Mathf.Sin(w*timeEllapsed+phase)*amp+yoffset;
            pos.y=animator.transform.position.y;
            animator.transform.position=pos;
            timeEllapsed+=Time.fixedDeltaTime;
            yield return wait;
        }
        animator.SetTrigger("toIdle");
    }
}
