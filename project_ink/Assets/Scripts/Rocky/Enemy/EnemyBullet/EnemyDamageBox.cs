using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageBox : EnemyBulletBase
{
    internal override void Start(){
        if(rgb==null)
            rgb=GetComponent<Rigidbody2D>();
    }
    internal override void OnTriggerEnter2D(Collider2D collider){
        InvokeOnTriggerEnterEvent(collider);
    }
}
