using UnityEngine;

public class MathUtil{
    public static float besierControlPointEffect=6;
    public static Vector2 Rotate(Vector2 dir, float theta){
        float sin=Mathf.Sin(theta), cos=Mathf.Cos(theta);
        return new Vector2(dir.x*cos-dir.y*sin, dir.x*sin+dir.y*cos);
    }
    public static Vector3 MultiplySeparately(Vector3 lhs, Vector3 rhs){
        return new Vector3(lhs.x*rhs.x, lhs.y*rhs.y, lhs.z*rhs.z);
    }
    public static Vector3 DivideSeparately(Vector3 lhs, Vector3 rhs){
        return new Vector3(lhs.x/rhs.x, lhs.y/rhs.y, lhs.z/rhs.z);
    }
    /// <summary>
    /// f(x)=sin(wx+a)
    /// </summary>
    public static float SwitchDirection(float w, float a, float x0)
    {
        float t=1/w*Mathf.PI*2;
        a-=2*((x0+a/w)%t)/t*6.28318f;
        return a;
    }
    /// <summary>
    /// Vector2.up rotated theta (in rad)
    /// </summary>
    /// <param name="theta">in rad</param>
    /// <returns></returns>
    public static Vector2 GetVector_up(float theta, Vector2 center, float radius){
        return new Vector2(-Mathf.Sin(theta)*radius, Mathf.Cos(theta)*radius)+center;
    }
    /// <summary>
    /// limit 'to' so that (to-from)'s magnitude squared is less than threshold
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="thresholdSqr"></param>
    /// <param name="maxMagnitude"></param>
    /// <returns></returns>
    public static Vector2 ClampFromToVector(Vector2 from, Vector2 to, float threshold){
        float thresholdSqr=threshold*threshold;
        Vector2 vectorFromTo=to-from;
        float distToTarget=vectorFromTo.x*vectorFromTo.x+vectorFromTo.y*vectorFromTo.y;
        if(distToTarget>thresholdSqr){
            to=(Vector2)from+vectorFromTo/Mathf.Sqrt(distToTarget)*threshold;
        }
        return to;
    }
    //---------------------noise------------------------
    static float IntToFLoatneg1pos1(int i){
        return ((float)i/int.MaxValue)*2-1f;
    }
    public static Vector2 InsideUnitCirclePerlinNoise(float t){
        int l=(int)t,r=l+1;
        System.Random lrandom=new System.Random(l), rrandom=new System.Random(r);
        float rand=Mathf.Lerp(IntToFLoatneg1pos1(lrandom.Next()), IntToFLoatneg1pos1(rrandom.Next()), t-l);
        float y=Mathf.Sqrt(1-rand*rand);
        return new Vector2(rand, IntToFLoatneg1pos1(lrandom.Next())>0?y:-y);
    }
    //---------------------besier curve------------------------
    /// <summary>
    /// interpolates a point based on quadratic besier curve
    /// </summary>
    public static Vector2 BesierQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        // Ensure t is in the range [0, 1]
        t = Mathf.Clamp01(t);
        // Calculate the quadratic Bézier curve point
        float u = 1 - t;
        float uSquared = u * u; // (1 - t)^2
        float tSquared = t * t; // t^2
        // B(t) = (1-t)^2 * p0 + 2(1-t)t * p1 + t^2 * p2
        return uSquared * p0 + 2 * u * t * p1 + tSquared * p2;
    }
    public static Vector2 BesierCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        // Ensure t is in the range [0, 1]
        t = Mathf.Clamp01(t);
        // Calculate the cubic Bézier curve point
        float u = 1 - t; // (1 - t)
        float uSquared = u * u; // (1 - t)^2
        float uCubed = uSquared * u; // (1 - t)^3
        float tSquared = t * t; // t^2
        float tCubed = tSquared * t; // t^3
        // B(t) = (1-t)^3 * p0 + 3(1-t)^2 * t * p1 + 3(1-t) * t^2 * p2 + t^3 * p3
        return uCubed * p0 + 3 * uSquared * t * p1 + 3 * u * tSquared * p2 + tCubed * p3;
    }
    /// <summary>
    /// returns generated curve given anchor points (p0 and p2). automatically generates control points (p1)
    /// </summary>
    public static Vector2[] BesierCubicCurve(Vector2[] anchors, int numPoints){
        //generate control points
        Vector2[] fullPoints=new Vector2[(anchors.Length-1)*3+1];
        for(int i=anchors.Length-2;i>0;--i){
            int i3=i*3;
            Vector2 dir=(anchors[i+1]-anchors[i-1])/besierControlPointEffect;
            fullPoints[i3]=anchors[i];
            fullPoints[i3+1]=anchors[i]+dir;
            fullPoints[i3-1]=anchors[i]-dir;
        }
        //calculate control point for starting and ending point
        fullPoints[0]=anchors[0];
        fullPoints[1]=anchors[0]+(fullPoints[2]-anchors[0])/besierControlPointEffect;
        fullPoints[^1]=anchors[^1];
        fullPoints[^2]=anchors[^1]+(fullPoints[^3]-anchors[^1])/besierControlPointEffect;
        //calculate curve
        Vector2[] res=new Vector2[numPoints*(anchors.Length-1)+1];
        int resIdx=0;
        int anchorsLength=anchors.Length-1;
        //Debug.Log($"anchors={anchors.Length}, fullpoints={fullPoints.Length}, res={res.Length}, numPoints={numPoints}");
        for(int i=0;i<anchorsLength;++i){
            int i3=i*3;
            for(int j=0;j<numPoints;++j){
                res[resIdx]=BesierCubic(fullPoints[i3],fullPoints[i3+1],fullPoints[i3+2],fullPoints[i3+3],(float)j/numPoints);
                ++resIdx;
            }
        }
        res[resIdx]=fullPoints[^1];
        return res;
    }
    //-------------physics----------------
    public static Vector2 CalcJumpVelocity(float startXPos, float endXPos, float jumpHeight, float gravity)
    {
        // Calculate the horizontal distance
        float deltaX = endXPos - startXPos;
        // Calculate the total time of flight
        float halfTime = Mathf.Sqrt(2*jumpHeight/gravity);
        // Calculate the initial vertical velocity
        float jumpYSpd = halfTime * gravity;
        float jumpXSpd=deltaX/halfTime*0.5f;

        // Return the initial velocity as a Vector2
        return new Vector2(jumpXSpd, jumpYSpd);
    }
}