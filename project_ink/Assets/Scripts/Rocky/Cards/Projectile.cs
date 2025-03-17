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
    [HideInInspector] public Card card;

    Coroutine coro;
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
    /// <summary>
    /// change the fly direction of the card
    /// </summary>
    /// <param name="dir">must be normalized</param>
    public void AdjustFlyDir(Vector2 dir){
        rgb.velocity=rgb.velocity.magnitude*dir;
    }
    public void InitProjectile(Card card, Vector2 pos, Vector2 velocity, ProjectileType type){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.card=card;
        damage=card.damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(coro!=null) StopCoroutine(coro);
        if(type==ProjectileType.AutoChase) coro=StartCoroutine(AutoChase());
        else if(type==ProjectileType.AutoFire) coro=StartCoroutine(AutoChase_AutoFire());
        //fire effect
        fireEffect.transform.position=transform.position;
        fireEffect.SetActive(true);
        Sequence s=DOTween.Sequence();
        s.AppendInterval(fireEffectDuration);
        s.AppendCallback(()=>fireEffect.SetActive(false));
        //trail effect
        trail.Clear();
        CardLog.CardProjectileInstantiated(card);
    }
    public void InitProjectile(int damage, Vector2 pos, Vector2 velocity, ProjectileType type){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.damage=damage;
        rgb.velocity=velocity;
        transform.position=pos;
        if(coro!=null) StopCoroutine(coro);
        if(type==ProjectileType.AutoChase) coro=StartCoroutine(AutoChase());
        else if(type==ProjectileType.AutoFire) coro=StartCoroutine(AutoChase_AutoFire());
        //fire effect
        fireEffect.transform.position=transform.position;
        fireEffect.SetActive(true);
        Sequence s=DOTween.Sequence();
        s.AppendInterval(fireEffectDuration);
        s.AppendCallback(()=>fireEffect.SetActive(false));
        //trail effect
        trail.Clear();
        CardLog.ProjectileInstantiated(this);
    }
    IEnumerator DelayDestroy(float seconds){
        yield return new WaitForSeconds(seconds);
        M_Destroy();
    }
    /// <summary>
    /// called by auto chase algorithm
    /// </summary>
    void ChaseEnemy_step(float angleConstraint){
        float spd=rgb.velocity.magnitude;
        Vector2 dir=rgb.velocity/spd, newDir;
        EnemyBase closestEnemy=RoomManager.inst.ClosestEnemy(transform);
        if(closestEnemy!=null){
            newDir=((Vector2)(closestEnemy.transform.position-transform.position)).normalized;
            float theta=Vector2.SignedAngle(dir, newDir);
            if(theta>angleConstraint || theta<-angleConstraint){
                theta=Mathf.Clamp(theta,-angleConstraint,angleConstraint);
                newDir=MathUtil.Rotate(dir, theta*Mathf.Deg2Rad);
            }
            rgb.velocity=spd*newDir;
            AdjustRotation(newDir);
        }
    }
    /// <summary>
    /// same as auto chase, but has a different auto fired effect
    /// </summary>
    /// <returns></returns>
    IEnumerator AutoChase_AutoFire(){
        WaitForSeconds wait=new WaitForSeconds(.04f);
        float toTime=Time.time+ProjectileManager.inst.af_time1;
        while(Time.time<=toTime){
            ChaseEnemy_step(ProjectileManager.inst.af_angleConstraint);
            yield return wait;
        }
        Vector2 oldVelocity=rgb.velocity;
        rgb.velocity=Vector2.zero;
        yield return new WaitForSeconds(ProjectileManager.inst.af_stoptime);
        rgb.velocity=oldVelocity;
        for(;;){
            ChaseEnemy_step(ProjectileManager.inst.autoChaseAngleConstraint);
            yield return wait;
        }
    }
    IEnumerator AutoChase(){
        WaitForSeconds wait=new WaitForSeconds(.04f);
        for(;;){
            ChaseEnemy_step(ProjectileManager.inst.autoChaseAngleConstraint);
            yield return wait;
        }
    }
    void M_Destroy(){
        if(gameObject.activeSelf==false) return; //eliminate repetitively calling on trigger enter
        if(coro!=null){
            StopCoroutine(coro);
            coro=null;
        }
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
    public enum ProjectileType{
        Normal,
        AutoChase,
        AutoFire,
    }
}
