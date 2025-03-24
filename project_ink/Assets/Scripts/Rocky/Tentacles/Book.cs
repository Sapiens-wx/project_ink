using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the object carrying a tentacle
/// </summary>
public class Book : MonoBehaviour
{
    public Tentacle tentacle;
    [Header("physics")]
    [SerializeField] float acceleration,damping;
    public float frq;

    [NonSerialized][HideInInspector] public int accumulatedDamage;
    float radius=1;
    float phase;
    Vector3 prevPos;
    void Start()
    {
        accumulatedDamage=0;
        phase=UnityEngine.Random.Range(0,200f);
    }

    void FixedUpdate()
    {
        FollowPlayer();
    }
    void FollowPlayer(){
        Vector3 tmp=transform.position;
        Vector3 dir=PlayerCtrl.inst.transform.position-tmp;
        Vector3 a=Vector3.zero;
        float dist=dir.magnitude;
        if(dist>radius){
            dir/=dist;
            a=(dist-radius)*acceleration*dir;
        } else{
            Vector2 rd=MathUtil.InsideUnitCirclePerlinNoise(Time.time+phase);
            a=rd*acceleration*frq;
        }
        transform.position+=(tmp-prevPos)*damping+a;
        prevPos=tmp;
    }
}
