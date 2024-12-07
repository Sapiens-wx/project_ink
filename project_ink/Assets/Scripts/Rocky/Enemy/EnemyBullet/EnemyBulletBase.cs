using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    public float spd;
    [HideInInspector] public Rigidbody2D rgb;
    internal virtual void Start(){
        if(rgb==null)
            rgb=GetComponent<Rigidbody2D>();
        Destroy(gameObject, 10);
    }
    internal virtual void OnTriggerEnter2D(Collider2D collider){
        Destroy(gameObject);
    }
}
