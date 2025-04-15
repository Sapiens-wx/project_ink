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
    public float frq;

    [NonSerialized][HideInInspector] public int accumulatedDamage;
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
        Vector3 tmp=transform.localPosition;
        Vector3 dir=-tmp;
        Vector3 a=Vector3.zero;
        float dist=dir.magnitude;
        if(dist>TentacleManager.inst.followRadius){
            dir/=dist;
            a=(dist-TentacleManager.inst.followRadius)*TentacleManager.inst.acceleration*dir;
        } else{
            Vector2 rd=MathUtil.InsideUnitCirclePerlinNoise(Time.time+phase);
            a=rd*TentacleManager.inst.acceleration*frq;
        }
        transform.localPosition+=(tmp-prevPos)*TentacleManager.inst.damping+a;
        prevPos=tmp;
    }
}
