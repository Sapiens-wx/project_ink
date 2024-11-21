using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LayerMask groundLayer; //relative to the enemy
    public static GameManager inst;
    void Awake()
    {
        inst=this;
    }
}
