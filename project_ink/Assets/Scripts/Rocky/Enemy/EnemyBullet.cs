using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour{
    public Rigidbody2D rgb;
    void Start(){
        Destroy(gameObject, 10);
    }
    void OnTriggerEnter2D(Collider2D collider){
        Destroy(gameObject);
    }
}