using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public LineRenderer line;
    public Transform anchorParent;
    //first point is the origin
    public Transform[] anchors;
    public float maxLength,len_idle;
    public int numPoints;
    public float besierCurveParams;
    [Header("Physics")]
    public float acceleration;
    public float colliderRadius;

    int damage;
    public int Damage{ get=>damage;}
    public event System.Action<EnemyBase> onHitEnemy;
    Animator animator;
    /// <summary>
    /// the length of the tentacle
    /// </summary>
    [NonSerialized][HideInInspector] public float len, len_attack;
    [NonSerialized][HideInInspector] public Vector2 target;
    /// <summary>
    /// actual anchor positions. with physical simulation, but also affected by anchors.
    /// </summary>
    Vector2[] positions, scaledPositions;
    Stack<Collider2D> hitStack;
    /// <summary>
    /// stores a vector3 p. p.xy is the attack position, p.z is the damage
    /// </summary>
    Queue<Vector3> attacks;
    int dir;
    public int Dir{
        get{
            return transform.lossyScale.x>=0?1:-1;
        }
        set{
            transform.localScale=MathUtil.DivideSeparately(MathUtil.MultiplySeparately(new Vector3(value,1,1),transform.localScale),transform.lossyScale);
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
    void Start()
    {
        Dir=1;
        len=len_idle;
        animator=GetComponent<Animator>();
        InitAnchorPos();
        hitStack=new Stack<Collider2D>();
        attacks=new Queue<Vector3>();
        damage=0;
    }
    /// <summary>
    /// initialize the positions array based on anchors array
    /// </summary>
    void InitAnchorPos(){
        positions=new Vector2[anchors.Length];
        for(int i=0;i<anchors.Length;++i){
            positions[i]=anchors[i].position;
        }
    }
    void FixedUpdate(){
        UpdateLine();
        DetectCollision();
    }
    public void Attack(Vector2 point, int _damage){
        attacks.Enqueue(new Vector3(point.x, point.y, _damage));
        animator.SetInteger("numAttacks", attacks.Count);
    }
    /// <summary>
    /// called by tentacle_attack_state only. gets the next target of attack. automatically sets damage to desired damage
    /// </summary>
    /// <returns></returns>
    public Vector2 GetNextAttack(){
        animator.SetInteger("numAttacks", attacks.Count-1);
        return attacks.Dequeue();
    }
    Vector2[] Scaled(Vector2[] arr, float length){
        Vector2[] res=new Vector2[arr.Length];
        Vector2 origin=arr[0];
        float originalLength=(arr[arr.Length-1]-origin).magnitude;
        float scale=length/originalLength;
        for(int i=res.Length-1;i>-1;--i)
            res[i]=origin+(arr[i]-origin)*scale;
        return res;
    }
    void UpdateLineRenderer(){
        if(len==0){
            line.positionCount=0;
            return;
        }
        scaledPositions=Scaled(positions, len);
        MathUtil.besierControlPointEffect=besierCurveParams;
        Vector2[] curve=MathUtil.BesierCubicCurve(scaledPositions, numPoints);
        if(line.positionCount!=curve.Length)
            line.positionCount=curve.Length;
        for(int i=curve.Length-1;i>-1;--i){
            line.SetPosition(i, curve[i]);
        }
    }
    /// <summary>
    /// update positions array based on physics simulation
    /// </summary>
    void UpdatePositions(){
        if(len==0) return;
        Vector2 a;
        for(int i=0;i<positions.Length;++i){
            a=((Vector2)anchors[i].position-positions[i])*Mathf.Lerp(1,acceleration,(float)i/positions.Length);
            positions[i]+=a;
        }
    }
    void UpdateLine(){
        if(anchors!=null && line!=null){
            UpdatePositions();
            UpdateLineRenderer();
        }
    }
    /// <summary>
    /// detects collision of a sphere centered at the last point of 'positions'
    /// </summary>
    void DetectCollision(){
        if(len==0){ //no collision detection
            while(hitStack.Count>0)
                OnCollExit(hitStack.Pop());
            return;
        }
        Debug.DrawLine(scaledPositions[^1]+new Vector2(-colliderRadius/2,0), scaledPositions[^1]+new Vector2(colliderRadius/2,0));
        Collider2D hit=Physics2D.OverlapCircle(scaledPositions[^1], colliderRadius, GameManager.inst.enemyLayer);
        if(hit==null){
            while(hitStack.Count>0)
                OnCollExit(hitStack.Pop());
            return;
        }
        if(hitStack.Count==0){ //on hit enter
            hitStack.Push(hit);
            OnCollEnter(hit);
        }
        else if(hitStack.Peek()!=hit){
            Collider2D top=hitStack.Pop();
            if(hitStack.Count==0 || hitStack.Peek()==hit){ //on hit exit, exit collider top
                OnCollExit(hit);
            }
            if(hitStack.Count==0 || hitStack.Peek()!=hit){ //on hit enter
                OnCollEnter(hit);
                hitStack.Push(top);
                hitStack.Push(hit);
            }
        } else if(hitStack.Peek()==hit){ //on hit stay
        }
    }
    void OnCollEnter(Collider2D collider){
        //Debug.Log("enter "+collider.name+Time.time);
        EnemyBase enemy=collider.GetComponent<EnemyBase>();
        if(enemy.CompareTag("IgnoreProjectile")) return; //if the hit collider ignores this projectile (like E_Pig does), then act as nothing happened.
        enemy.OnHit(new HitEnemyInfo(this));
        onHitEnemy?.Invoke(enemy);
    }
    void OnCollExit(Collider2D collider){
        //Debug.Log("exit "+collider.name+Time.time);
    }
}
