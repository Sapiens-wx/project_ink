using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public LineRenderer line;
    //first point is the origin
    public Transform[] anchors;
    public float len_idle,len_attack;

    [NonSerialized][HideInInspector] public float len;
    void OnValidate(){
        UpdateLine();
    }
    void Start()
    {
        len=len_idle;
    }
    void FixedUpdate(){
        UpdateLine();
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
