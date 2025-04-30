using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class B1_1_Scroll : StateBase<B1_1_Ctrller>
{
    [SerializeField] float toRightMostSpeed;
    [Header("Action1")]
    [SerializeField] float a1_loopDuration;
    [Header("Action2")]
    [SerializeField] float a2_duration;

    float prevX;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        prevX=ctrller.transform.position.x;
        BeginRandomAction(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ctrller.Dir=ctrller.transform.position.x>prevX?1:-1;
        prevX=ctrller.transform.position.x;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    void BeginRandomAction(Animator animator){
        float rightMost=RoomManager.CurrentRoom.RoomBounds.max.x;
        float dist=rightMost-ctrller.transform.position.x;
        //move to the right-most position of the room
        Sequence s = DOTween.Sequence();
        s.Append(ctrller.transform.DOMoveX(rightMost, dist/toRightMostSpeed));
        if(Random.Range(0,2)==0){ //action 1 (on the ground)
            s.AppendCallback(()=>Action1(animator));
        } else{
            s.AppendCallback(()=>Action2(animator));
        }
    }
    void Action1(Animator animator){
        float leftMost=RoomManager.CurrentRoom.RoomBounds.min.x;
        var scrollAnim = ctrller.transform.DOMoveX(leftMost, a1_loopDuration).SetEase(Ease.InOutQuad).SetLoops(-1,LoopType.Yoyo);
        Sequence s=DOTween.Sequence();
        s.AppendInterval(5);
        s.AppendCallback(()=>{scrollAnim.Kill(); animator.SetTrigger("toIdle");});
        s.Play();
    }
    //action 2
    void Action2(Animator animator){
        Vector3 originalPos=ctrller.transform.position;
        float leftMost=RoomManager.CurrentRoom.RoomBounds.min.x;

        Sequence s=DOTween.Sequence();
        s.Append(ctrller.transform.DOMoveX(leftMost, a2_duration).SetEase(Ease.Linear));
        s.Join(ctrller.transform.DOMoveY(ctrller.a1_2_jumpHeight, a2_duration/4).SetLoops(4, LoopType.Yoyo).SetEase(Ease.OutExpo));
        s.AppendCallback(()=>{
            animator.SetTrigger("toIdle");
        });
        s.Play();
    }
}
