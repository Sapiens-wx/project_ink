using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_PlayerStatusDetector : Singleton<B1_PlayerStatusDetector>{
    /// <summary>
    /// whether the player is on the platform or on the ground
    /// </summary>
    private bool isPlayerOnPlatform;
    public bool IsPlayerOnPlatform{
        get=>isPlayerOnPlatform;
    }
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player"))
            isPlayerOnPlatform=true;
    }
    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player"))
            isPlayerOnPlatform=false;
    }
}