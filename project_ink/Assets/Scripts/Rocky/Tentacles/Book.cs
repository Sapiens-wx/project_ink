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

    float radius=1;
    Vector3 prevPos;
    void Start()
    {
        
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
        }
        transform.position+=(tmp-prevPos)*damping+a;
        prevPos=tmp;
    }
}
