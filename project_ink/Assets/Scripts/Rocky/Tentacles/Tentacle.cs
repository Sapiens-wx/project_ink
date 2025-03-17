using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Tentacle : Singleton<Tentacle>
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

    Animator animator;
    /// <summary>
    /// the length of the tentacle
    /// </summary>
    [NonSerialized][HideInInspector] public float len,len_attack;
    [NonSerialized][HideInInspector] public Vector2 target;
    /// <summary>
    /// actual anchor positions. with physical simulation, but also affected by anchors.
    /// </summary>
    Vector2[] positions;
    int dir;
    public int Dir{
        get{
            return transform.lossyScale.x>=0?1:-1;
        }
        set{
            transform.localScale=MathUtil.DivideSeparately(MathUtil.MultiplySeparately(new Vector3(value,1,1),transform.localScale),transform.lossyScale);
        }
    }
    void OnValidate(){
        //UpdateLine();
    }
    void Start()
    {
        Dir=1;
        len=len_idle;
        animator=GetComponent<Animator>();
        InitAnchorPos();
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
    }
    public void Attack(Vector2 point){
        target=point;
        animator.SetTrigger("attack");
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
        if(len<=Mathf.Epsilon){
            line.positionCount=0;
            return;
        }
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
            positions[i]+=a;
        }
    }
    void UpdateLine(){
        if(anchors!=null && line!=null){
            UpdatePositions();
            UpdateLineRenderer();
        }
    }
}
