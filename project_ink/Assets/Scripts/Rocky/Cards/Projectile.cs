using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject fireEffect;
    [SerializeField] TrailRenderer trail;
    [SerializeField] float fireEffectDuration;
    [HideInInspector] public Rigidbody2D rgb;
    [HideInInspector] public int damage;
    [HideInInspector] public bool chase;
    [HideInInspector] public Card card;

    // Start is called before the first frame update
    void Start()
    {
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        //detech fire effect game object from its parent
        fireEffect.transform.SetParent(null);
    }
    void OnEnable(){
        StartCoroutine(DelayDestroy(5));
    }
    void OnDisable(){
        StopAllCoroutines();
        if(fireEffect!=null)
            fireEffect.SetActive(false);
    }
    public void AdjustRotation(Vector2 dir){
        transform.rotation=Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, dir), Vector3.forward);
    }
    public void InitProjectile(Card card, Vector2 pos, Vector2 velocity, bool chase){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.card=card;
        damage=card.damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(chase) StartCoroutine(AutoChase());
        //fire effect
        fireEffect.transform.position=transform.position;
        fireEffect.SetActive(true);
        Sequence s=DOTween.Sequence();
        s.AppendInterval(fireEffectDuration);
        s.AppendCallback(()=>fireEffect.SetActive(false));
        //trail effect
        trail.Clear();
    }
    public void InitProjectile(int damage, Vector2 pos, Vector2 velocity, bool chase){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.damage=damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(chase) StartCoroutine(AutoChase());
        //fire effect
        fireEffect.transform.position=transform.position;
        fireEffect.SetActive(true);
        Sequence s=DOTween.Sequence();
        s.AppendInterval(fireEffectDuration);
        s.AppendCallback(()=>fireEffect.SetActive(false));
        //trail effect
        trail.Clear();
    }
    IEnumerator DelayDestroy(float seconds){
        yield return new WaitForSeconds(seconds);
        M_Destroy();
    }
    IEnumerator AutoChase(){
        WaitForSeconds wait=new WaitForSeconds(.04f);
        Vector2 newDir, dir;
        for(;;){
            float spd=rgb.velocity.magnitude;
            dir=rgb.velocity/spd;
            newDir=((Vector2)(RoomManager.inst.ClosestEnemy(transform).transform.position-transform.position)).normalized;
            float theta=Vector2.SignedAngle(dir, newDir);
            if(theta>17f || theta<-17f){
                theta=Mathf.Clamp(theta,-17f,17f);
                newDir=MathUtil.Rotate(dir, theta*Mathf.Deg2Rad);
            }
            rgb.velocity=spd*newDir;
            AdjustRotation(newDir);
            yield return wait;
        }
    }
    void M_Destroy(){
        if(gameObject.activeSelf==false) return; //eliminate repetitively calling on trigger enter
        StopAllCoroutines();
        ProjectileManager.inst.ReleaseProjectile(this);
    }
    void OnTriggerEnter2D(Collider2D collider){
        if(GameManager.IsLayer(GameManager.inst.enemyLayer, collider.gameObject.layer)){ //if is enemy
            EnemyBase enemy=collider.GetComponent<EnemyBase>();
            enemy.OnHit(this);
            if(card!=null) card.OnHitEnemy(enemy);
        }
        ProjectileManager.inst.HitAnim(this);
        M_Destroy();
    }
}
