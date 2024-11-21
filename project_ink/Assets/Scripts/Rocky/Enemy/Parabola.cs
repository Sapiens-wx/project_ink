using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    public bool update;
    public Transform fromT,toT;
    public float h_by_y;

    float a,b,c;
    Vector2 from,to;
    float xDist,yDist;
    float h, k;
    void OnDrawGizmos(){
        if(fromT==null || toT==null) return;
        if(update) OnValidate();
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(new Vector2(h,k),.4f);
    }

    [ContextMenu("update")]
    void OnValidate(){
        from=fromT.transform.position;
        to=toT.transform.position;
        float x1=from.x,y1=from.y,x2=to.x,y2=to.y;
        xDist=to.x-from.x;
        yDist=to.y-from.y;

        h=x1+xDist/2;
        h+=Mathf.Atan(yDist/h_by_y)/Mathf.PI*xDist;

        a=(y2-y1)/((x2-x1)*(x2+x1-2*h));
        k=y1-((y2-y1)*(x1-h)*(x1-h) / ((x2-x1)*(x2+x1-2*h)));
    }
    public Vector2 GetVelocity(Vector2 from, Vector2 to, float g){
        return Vector2.zero;
    }
}
