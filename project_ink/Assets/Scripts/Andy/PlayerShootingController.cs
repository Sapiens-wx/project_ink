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
                Instantiate(cards[index], shotPoint.position, transform.rotation);
                index++;
                if (index >= cards.Length)
                {
                    attackAnim = true;
                    //shuffle if card used all
                    //Card card=CardSlotManager.GetCard();
                    //Shoot(card);
                    index = 0;
                }

                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }

        if (attackAnim)
        {
            if (playerController.isGround)
            {
                myAnim.SetBool("Attack", true);
            }
            else
            {
                myAnim.SetBool("AttackInAir", true);
            }
            myAnim.SetBool("Attack", false);
            myAnim.SetBool("AttackInAir", false);
        }
    }

    public void Shoot(Card card)
    {
        //card.
    }
}
