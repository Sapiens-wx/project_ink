using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public int damage;
    [HideInInspector] public bool chase;
    // Start is called before the first frame update
    void Start()
    {
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
    }
    void OnEnable(){
        StartCoroutine(DelayDestroy(5));
    }
    public void AdjustRotation(Vector2 dir){
        transform.rotation=Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, dir), Vector3.forward);
    }
    public void InitProjectile(Card card, Vector2 pos, Vector2 velocity, bool chase){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        damage=card.damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(chase) StartCoroutine(AutoChase());
    }
    public void InitProjectile(int damage, Vector2 pos, Vector2 velocity, bool chase){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.damage=damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(chase) StartCoroutine(AutoChase());
    }
    IEnumerator DelayDestroy(float seconds){
        yield return new WaitForSeconds(seconds);
        M_Destroy();
    }
    IEnumerator AutoChase(){
        WaitForSeconds wait=new WaitForSeconds(.08f);
        Vector2 dir;
        for(;;){
            dir=((Vector2)(RoomManager.inst.ClosestEnemy(transform).transform.position-transform.position)).normalized;
            rgb.velocity=rgb.velocity.magnitude*dir;
            AdjustRotation(dir);
            yield return wait;
        }
    }
    void M_Destroy(){
        ProjectileManager.inst.ReleaseProjectile(this);
    }
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.layer==8){ //if is enemy
            EnemyBase enemy=collider.GetComponent<EnemyBase>();
            enemy.OnHit(this);
        }
        M_Destroy();
    }
}
