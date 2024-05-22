using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class B_RolfController : MonoBehaviour
{
    public int activateDistance;
    public Transform player;
    private Animator animator;
    private List<string> states;
    private List<string> originalStateOne = new List<string> { "Dash", "Flower", "Cake" };

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
            animator.SetBool("Activate",true);
        }
    }
    public void RandomState()
    {
        if (states.Count == 0)
        {
            ResetAndShuffleStates();
        }

        string randomState = states[0];
        states.RemoveAt(0);

        // Assuming you have triggers named after the strings in originalStates in the Animator
        animator.SetTrigger(randomState);

        Debug.Log("Picked state: " + randomState);
    }

    void ResetAndShuffleStates()
    {
        states = new List<string>(originalStateOne);
        Shuffle(states);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
