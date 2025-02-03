using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingController : Singleton<PlayerShootingController>
{
    public GameObject[] cards;
    public float offset;
    public Transform shotPoint;
    private float timeBtwShots;
    public float startTimeBtwShots;
    public Animator ArmAnimator;

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
                //Instantiate(cards[index], shotPoint.position, transform.rotation);
                attackAnim = true;

                timeBtwShots = startTimeBtwShots;
                ArmAnimator.gameObject.SetActive(true);
                ArmAnimator.SetTrigger("Attack");
                ArmAnimator.SetFloat("AttackValue", 0);
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }

        if(Input.GetMouseButtonDown(1)){
            CardSlotManager.inst.SkipCard();
        }
    }

    public void Shoot(Card card)
    {
        //card.
    }
}
