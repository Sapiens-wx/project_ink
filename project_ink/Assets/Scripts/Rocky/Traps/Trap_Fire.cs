using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Fire : TrapBase
{
    [SerializeField] float xspd;
    [SerializeField] Bounds trail;
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(trail.center, trail.size);
    }
    protected override void Start(){
        base.Start();
        transform.position=trail.min;
        StartCoroutine(Rotate());
    }
    IEnumerator Rotate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        Vector2[] points=new Vector2[]{trail.min,new Vector2(trail.max.x, trail.min.y), trail.max, new Vector2(trail.min.x, trail.max.y)};
        int i=0,j=0;
        float toTime=0,t=0;
        Vector3 dir=Vector3.zero;
        while(true){
            if(t>=toTime){
                transform.position=points[j];
                i=j;
                j=(j+1)%points.Length;
                dir=points[j]-points[i];
                float mag=dir.magnitude;
                dir=dir/mag*xspd*Time.fixedDeltaTime;
                toTime=mag/xspd;
                t=0;
            } else{
                transform.position+=dir;
                t+=Time.fixedDeltaTime;
            }
            yield return wait;
        }
    }
}
