using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rgb;
    int damage;
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
    public void InitProjectile(Card card, Vector2 pos, Vector2 velocity){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        damage=card.damage;
        rgb.velocity=velocity;
        transform.position=pos;
    }
    public void InitProjectile(int damage, Vector2 pos, Vector2 velocity){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.damage=damage;
        rgb.velocity=velocity;
        transform.position=pos;
    }
    IEnumerator DelayDestroy(float seconds){
        yield return new WaitForSeconds(seconds);
        M_Destroy();
    }
    void M_Destroy(){
        ProjectileManager.inst.ReleaseProjectile(this);
    }
    void OnTriggerEnter2D(Collider2D collider){
        EnemyBase enemy=collider.GetComponent<EnemyBase>();
        enemy.OnHit(this);
        M_Destroy();
    }
}
