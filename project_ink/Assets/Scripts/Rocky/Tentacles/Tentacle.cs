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

    Animator animator;
    [NonSerialized][HideInInspector] public float len;
    void OnValidate(){
        UpdateLine();
    }
    void Start()
    {
        len=len_idle;
        animator=GetComponent<Animator>();
    }
    void FixedUpdate(){
        UpdateLine();
    }
    void Update(){
        if(Input.GetMouseButtonDown(0)){
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
    Vector3[] ToVectors(float length){
        Vector3[] res=new Vector3[anchors.Length];
        for(int i=res.Length-1;i>-1;--i)
            res[i]=anchors[i].position;
        Vector3 origin=res[0];
        float originalLength=(res[res.Length-1]-origin).magnitude;
        float scale=length/originalLength;
        for(int i=res.Length-1;i>-1;--i){
            res[i]=origin+(res[i]-origin)*scale;
        }
        return res;
    }
    void UpdateLine(){
        if(anchors!=null && line!=null){
            if(line.positionCount!=anchors.Length)
                line.positionCount=anchors.Length;
            line.SetPositions(ToVectors(len));
        }
    }
}
