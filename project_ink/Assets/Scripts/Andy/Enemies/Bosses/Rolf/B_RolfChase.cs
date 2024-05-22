using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_RolfChase : StateMachineBehaviour
{
    public float chaseDuration ;
    public float chaseSpeed;
    private Transform player;
    private float chaseTimer;
    private Rigidbody2D rb;
    private B_RolfController rolfController;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        chaseTimer = chaseDuration;
        rb = animator.GetComponent<Rigidbody2D>();
        rolfController = animator.GetComponent<B_RolfController>();
    }

     //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chaseTimer -= Time.deltaTime;

        if (chaseTimer > 0)
        {
            Vector2 target = new Vector2 (player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, chaseSpeed);
            //flipping function
            rb.MovePosition(newPos);
        }
        else
        {
            Debug.Log("Chasing stop");
            //rolfController.RandomState();
        }
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
