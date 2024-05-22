using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingController : MonoBehaviour
{
    public GameObject[] cards;
    public float offset;
    public Transform shotPoint;
    private float timeBtwShots;
    public float startTimeBtwShots;

    public enum LOOKING_DIRECTION
    {
        UP,
        FORWARD_UP,
        FORWARD,
        FORWARD_DOWN,
        DOWN,
    }



    private int index; 
    private Animator myAnim;
    private PlayerController playerController;
    private bool attackAnim = false;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        myAnim = GetComponent<Animator>();

    }
    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print(GetCurrentLookingDirection());
                //Instantiate(cards[index], shotPoint.position, transform.rotation);
                attackAnim = true;

                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }

        if (attackAnim)
        {
            Debug.Log("Attacked");
            LOOKING_DIRECTION direction = GetCurrentLookingDirection();
            if (playerController.isGround)
            {
                if (direction == LOOKING_DIRECTION.UP)
                {
                    myAnim.SetTrigger("Attack_U");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD_UP)
                {
                    myAnim.SetTrigger("Attack_FU");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD)
                {
                    myAnim.SetTrigger("Attack_F");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD_DOWN)
                {
                    myAnim.SetTrigger("Attack_FD");
                }
                else if (direction == LOOKING_DIRECTION.DOWN)
                {
                    myAnim.SetTrigger("Attack_D");
                }
            }
            else
            {
                if (direction == LOOKING_DIRECTION.UP)
                {
                    myAnim.SetTrigger("AttackInAir_U");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD_UP)
                {
                    myAnim.SetTrigger("AttackInAir_FU");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD)
                {
                    myAnim.SetTrigger("AttackInAir_F");
                }
                else if (direction == LOOKING_DIRECTION.FORWARD_DOWN)
                {
                    myAnim.SetTrigger("AttackInAir_FD");
                }
                else if (direction == LOOKING_DIRECTION.DOWN)
                {
                    myAnim.SetTrigger("AttackInAir_D");
                }
            }
           // myAnim.SetBool("Attack", false);
            //myAnim.SetBool("AttackInAir", false);
            attackAnim = false;
        }
    }

    public LOOKING_DIRECTION GetCurrentLookingDirection()
    {
        Vector2 mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        Vector2 forward = PlayerController.CurrentFacingDirection == PlayerController.FACING_DIRECTION.LEFT? Vector2.left : Vector2.right;
        float angle = Vector2.Angle(forward, mouseDirection);
        if(angle<=22.5f)
        {
            return LOOKING_DIRECTION.FORWARD;
        }
        else if(angle<=67.5)
        {
            return mouseDirection.y < 0 ? LOOKING_DIRECTION.FORWARD_DOWN : LOOKING_DIRECTION.FORWARD_UP;
        }
        else
        {
            return mouseDirection.y < 0 ? LOOKING_DIRECTION.DOWN : LOOKING_DIRECTION.UP;
        }
    }

    public void Shoot(Card card)
    {
        //card.
    }
}
