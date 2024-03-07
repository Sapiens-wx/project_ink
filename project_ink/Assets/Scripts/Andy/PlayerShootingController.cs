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

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(cards[index], shotPoint.position, transform.rotation);
                index++;
                if (index >= cards.Length)
                {
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
    }

    public void Shoot(Card card)
    {
        //card.
    }
}
