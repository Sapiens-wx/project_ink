using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_RolfController : MonoBehaviour
{
    public int activateDistance;
    public Transform player;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(transform.position, player.position)) <= activateDistance)
        {
            Debug.Log("Activate");
            animator.SetBool("Activate",true);
        }
    }
}
