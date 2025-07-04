using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject fireEffect;
    [SerializeField] TrailRenderer trail;
    [SerializeField] float fireEffectDuration;
    [HideInInspector] public int damage;
    [HideInInspector] public Card card;
    [HideInInspector] public Vector2 velocity, trap_v_fan;

    ProjectileType projectileType;
    Rigidbody2D rgb;
    Collider2D bc;
    Coroutine coro;
    void Awake(){
        rgb=GetComponent<Rigidbody2D>();
        bc=GetComponent<Collider2D>();
        if(bc==null||!(bc is BoxCollider2D))
            Debug.LogWarning("projectile's collider2D component is not box. might cause errors");
        //detech fire effect game object from its parent
        fireEffect.transform.SetParent(null);
    }
    // Start is called before the first frame update
    void OnEnable(){
        StartCoroutine(DelayDestroy(5));
    }
    void OnDisable(){
        StopAllCoroutines();
        if(fireEffect!=null)
            fireEffect.SetActive(false);
    }
    void FixedUpdate(){
        rgb.velocity=velocity+trap_v_fan;
        //if is not auto fired card, destroy it when colliding with groundLayer walls
        if(projectileType!=ProjectileType.AutoFire&&Physics2D.OverlapBox(bc.bounds.center, bc.bounds.size, transform.eulerAngles.z, GameManager.inst.groundLayer)){
            ProjectileManager.inst.HitAnim(this);
            M_Destroy();
        }
    }
    public void AdjustRotation(Vector2 dir){
        transform.rotation=Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, dir), Vector3.forward);
    }
    /// <summary>
    /// change the fly direction of the card
    /// </summary>
    /// <param name="dir">must be normalized</param>
    public void AdjustFlyDir(Vector2 dir){
        velocity=velocity.magnitude*dir;
    }
    public void InitProjectile(Card card, Vector2 pos, Vector2 velocity, ProjectileType type){
        if(rgb==null) rgb=GetComponent<Rigidbody2D>();
        this.card=card;
        damage=card.damage;
        this.velocity=velocity;
        this.projectileType=type;
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
        this.velocity=velocity;
        this.projectileType=type;
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
        float spd=this.velocity.magnitude;
        Vector2 dir=this.velocity/spd, newDir;
        EnemyBase closestEnemy=RoomManager.inst.ClosestEnemy(transform);
        if(closestEnemy!=null){
            newDir=((Vector2)(closestEnemy.transform.position-transform.position)).normalized;
            float theta=Vector2.SignedAngle(dir, newDir);
            if(theta>angleConstraint || theta<-angleConstraint){
                theta=Mathf.Clamp(theta,-angleConstraint,angleConstraint);
                newDir=MathUtil.Rotate(dir, theta*Mathf.Deg2Rad);
            }
            this.velocity=spd*newDir;
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
        Vector2 oldVelocity=this.velocity;
        this.velocity=Vector2.zero;
        yield return new WaitForSeconds(ProjectileManager.inst.af_stoptime);
        this.velocity=oldVelocity;
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
            if(enemy.CompareTag("IgnoreProjectile")) return; //if the hit collider ignores this projectile (like E_Pig does), then act as nothing happened.
            enemy.OnHit(new HitEnemyInfo(this));
            if(card!=null) card.OnHitEnemy(enemy);
        }
        if(GameManager.IsLayer(GameManager.inst.projectileDestroyLayer, collider.gameObject.layer)){
            ProjectileManager.inst.HitAnim(this);
            M_Destroy();
        }
    }
    public enum ProjectileType{
        Normal,
        AutoChase,
        AutoFire,
    }
}
public class HitEnemyInfo{
    public HitType hitType;
    public Projectile projectile;
    public Transform transform;
    public int damage;
    public HitEnemyInfo(Projectile p){
        transform=p.transform;
        hitType=HitType.Projectile;
        damage=p.damage;
    }
    public HitEnemyInfo(Tentacle t){
        transform=t.transform;
        hitType=HitType.Tentacle;
        damage=t.Damage;
    }
    public enum HitType{
        Projectile,
        Tentacle
    }
}