using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    public float spd;
    public int damage=1;
    [HideInInspector] public Rigidbody2D rgb;
    public event System.Action<EnemyBulletBase,Collider2D> onTriggerEnter;
    internal virtual void Start(){
        if(rgb==null)
            rgb=GetComponent<Rigidbody2D>();
        Destroy(gameObject, 10);
    }
    internal virtual void OnTriggerEnter2D(Collider2D collider){
        onTriggerEnter?.Invoke(this, collider);
        StartCoroutine(DelayDestroy());
    }
    protected void InvokeOnTriggerEnterEvent(Collider2D collider){
        onTriggerEnter?.Invoke(this, collider);
    }
    IEnumerator DelayDestroy(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        yield return wait;
        yield return wait;
        Destroy(gameObject);
    }
}
