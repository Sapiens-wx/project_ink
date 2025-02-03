using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LayerMask groundLayer; //relative to the enemy
    public LayerMask enemyLayer;
    public LayerMask enemyBulletLayer;
    public float distEpsilon;
    public static GameManager inst;
    void Awake()
    {
        inst=this;
    }
    public static bool IsLayer(LayerMask mask, int layer){
        return (mask.value&(1<<layer))!=0;
    }
}
