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
    protected void Start(){
        StartCoroutine(Rotate());
    }
    IEnumerator Rotate(){
        WaitForFixedUpdate wait=new WaitForFixedUpdate();
        Vector2[] points=new Vector2[]{trail.min,new Vector2(trail.max.x, trail.min.y), trail.max, new Vector2(trail.min.x, trail.max.y)};
        int i=0,j=0;
        float toTime=0,t=0;
        Vector3 dir;
        //start from the position of the fire, instead of reseting the fire to the left bottom.
        Vector2 fireStartDir=transform.position-trail.center;
        float h2wRatio=trail.size.y/trail.size.x, actualRatio=Mathf.Abs(fireStartDir.y/fireStartDir.x);
        if(actualRatio>=h2wRatio){
            if(fireStartDir.y>=0){//start in upper triangle
                j=3;
                dir=Vector2.left;
                transform.position=Vector2.Dot((Vector2)transform.position-points[2], dir)*Vector2.left+points[2];
            } else{ //start in lower triangle
                j=1;
                dir=Vector2.right;
                transform.position=Vector2.Dot((Vector2)transform.position-points[0], dir)*Vector2.right+points[0];
            }
        } else if(fireStartDir.x>=0){//start in right triangle
            j=2;
            dir=Vector2.up;
            transform.position=Vector2.Dot((Vector2)transform.position-points[1], dir)*Vector2.up+points[1];
        } else{//start in left triangle
            j=0;
            dir=Vector2.down;
            transform.position=Vector2.Dot((Vector2)transform.position-points[3], dir)*Vector2.down+points[3];
        }
        dir*=xspd*Time.fixedDeltaTime;
        toTime=Vector2.Distance(transform.position, points[j])/xspd;
        while(t<toTime){
            transform.position+=dir;
            t+=Time.fixedDeltaTime;
            yield return wait;
        }
        //begin the loop
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
