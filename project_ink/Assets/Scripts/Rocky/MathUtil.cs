using UnityEngine;

public class MathUtil{
    public static Vector2 Rotate(Vector2 dir, float theta){
        float sin=Mathf.Sin(theta), cos=Mathf.Cos(theta);
        return new Vector2(dir.x*cos-dir.y*sin, dir.x*sin+dir.y*cos);
    }
    /// <summary>
    /// f(x)=sin(wx+a)
    /// </summary>
    public static float SwitchDirection(float w, float a, float x0)
    {
        float t=1/w*Mathf.PI*2;
        a-=2*((x0+a/w)%t)/t*6.28318f;
        return a;
        // Compute the new phase shift
        float aNew = a + Mathf.PI - 2 * (w * x0 + a);

        // Normalize aNew to the range [0, 2π) to keep it small
        aNew = aNew % (2 * Mathf.PI);
        if (aNew < 0) {
            aNew += 2 * Mathf.PI;
        }

        return aNew;
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