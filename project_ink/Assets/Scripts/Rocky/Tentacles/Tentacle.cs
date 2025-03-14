using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public LineRenderer line;
    public Transform anchorParent;
    //first point is the origin
    public Transform[] anchors;
    public float maxLength,len_idle,len_attack;
    public int numPoints;
    public float besierCurveParams;
    [Header("Physics")]
    public float acceleration;

    Animator animator;
    /// <summary>
    /// the length of the tentacle
    /// </summary>
    [NonSerialized][HideInInspector] public float len;
    /// <summary>
    /// actual anchor positions. with physical simulation, but also affected by anchors.
    /// </summary>
    Vector2[] positions, positions_prev;
    void OnValidate(){
        //UpdateLine();
    }
    void Start()
    {
        len=len_idle;
        animator=GetComponent<Animator>();
        InitAnchorPos();
    }
    /// <summary>
    /// initialize the positions array based on anchors array
    /// </summary>
    void InitAnchorPos(){
        positions=new Vector2[anchors.Length];
        positions_prev=new Vector2[anchors.Length];
        for(int i=0;i<anchors.Length;++i){
            positions[i]=anchors[i].position;
            positions_prev[i]=anchors[i].position;
        }
    }
    void FixedUpdate(){
        UpdateLine();
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.F)){
            Attack(PlayerCtrl.inst.mouseWorldPos);
        }
    }
    public void Attack(Vector2 point){
        Vector2 dir=point-(Vector2)anchors[0].transform.position;
        float length=Mathf.Min(dir.magnitude,maxLength);
        dir/=length;
        if(point.x<anchors[0].position.x){
            dir=MathUtil.Rotate(dir, -Mathf.PI/2);
            anchorParent.localScale=new Vector3(1,1,1);
        } else{
            dir=MathUtil.Rotate(dir, Mathf.PI/2);
            anchorParent.localScale=new Vector3(-1,1,1);
        }
        animator.SetTrigger("attack");
        len_attack=length;
        anchorParent.eulerAngles=new Vector3(0,0,Vector2.SignedAngle(Vector2.up, dir));
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
        Vector2[] scaledPositions=Scaled(positions, len);
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
        Vector2 a;
        for(int i=0;i<positions.Length;++i){
            a=((Vector2)anchors[i].position-positions[i])*Mathf.Lerp(1,acceleration,(float)i/positions.Length);
            positions[i]=positions[i]*2-positions_prev[i]+a;
            positions_prev[i]=positions[i];
        }
    }
    void UpdateLine(){
        if(anchors!=null && line!=null){
            UpdatePositions();
            UpdateLineRenderer();
        }
    }
}
