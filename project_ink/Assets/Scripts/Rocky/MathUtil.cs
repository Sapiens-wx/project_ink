using UnityEngine;

public static class MathUtil{
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
    /// <summary>
    /// given a 2D ray and bounds, calculate the normal of the point where they intersect. ray and bounds must intersect
    /// </summary>
    public static Vector3 CalculateIntersectionNormal(Ray ray, Bounds bounds)
    {
        // 确保是 2D 情况（z=0）
        Vector3 origin = ray.origin;
        Vector3 direction = ray.direction;
        origin.z = 0;
        direction.z = 0;

        // 提前计算方向分量的倒数，避免重复除法
        Vector3 invDirection = new Vector3(
            1.0f / direction.x,
            1.0f / direction.y,
            0
        );

        // 初始化最小 t 值和法线
        float tMin = float.NegativeInfinity;
        float tMax = float.PositiveInfinity;
        Vector3 normal = Vector3.zero;

        // 检查 X 轴
        if (float.IsInfinity(invDirection.x))
        {
            // 射线平行于 X 轴
            if (origin.x < bounds.min.x || origin.x > bounds.max.x)
            {
                // 不相交（但题目保证相交，所以这里不会执行）
                return Vector3.zero;
            }
        }
        else
        {
            float t1 = (bounds.min.x - origin.x) * invDirection.x;
            float t2 = (bounds.max.x - origin.x) * invDirection.x;

            if (t1 > t2)
            {
                (t1, t2) = (t2, t1); // 交换使 t1 <= t2
                normal.x = -1; // 可能命中 max.x 面
            }
            else
            {
                normal.x = 1; // 可能命中 min.x 面
            }

            tMin = Mathf.Max(tMin, t1);
            tMax = Mathf.Min(tMax, t2);

            if (tMin > tMax)
            {
                // 不相交（但题目保证相交，所以这里不会执行）
                return Vector3.zero;
            }
        }

        // 检查 Y 轴
        if (float.IsInfinity(invDirection.y))
        {
            // 射线平行于 Y 轴
            if (origin.y < bounds.min.y || origin.y > bounds.max.y)
            {
                // 不相交（但题目保证相交，所以这里不会执行）
                return Vector3.zero;
            }
        }
        else
        {
            float t1 = (bounds.min.y - origin.y) * invDirection.y;
            float t2 = (bounds.max.y - origin.y) * invDirection.y;

            if (t1 > t2)
            {
                (t1, t2) = (t2, t1); // 交换使 t1 <= t2
                normal.y = -1; // 可能命中 max.y 面
            }
            else
            {
                normal.y = 1; // 可能命中 min.y 面
            }

            tMin = Mathf.Max(tMin, t1);
            tMax = Mathf.Min(tMax, t2);

            if (tMin > tMax)
            {
                // 不相交（但题目保证相交，所以这里不会执行）
                return Vector3.zero;
            }
        }

        // 确定最终的法线方向
        // 我们需要找出是哪个轴的面导致了相交（tMin对应的面）
        float tx1 = (bounds.min.x - origin.x) * invDirection.x;
        float tx2 = (bounds.max.x - origin.x) * invDirection.x;
        float ty1 = (bounds.min.y - origin.y) * invDirection.y;
        float ty2 = (bounds.max.y - origin.y) * invDirection.y;

        // 找出哪个t值等于tMin
        if (Mathf.Approximately(tMin, tx1))
        {
            return Vector3.right; // 命中 min.x 面，法线向右
        }
        else if (Mathf.Approximately(tMin, tx2))
        {
            return Vector3.left; // 命中 max.x 面，法线向左
        }
        else if (Mathf.Approximately(tMin, ty1))
        {
            return Vector3.up; // 命中 min.y 面，法线向上
        }
        else if (Mathf.Approximately(tMin, ty2))
        {
            return Vector3.down; // 命中 max.y 面，法线向下
        }

        // 如果由于浮点精度未能精确匹配，则根据tMin的来源判断
        // 这种情况在理论上不应该发生，因为题目保证相交
        return normal;
    }
    public static bool IntersectPoint(this Bounds lhs, Vector2 point){
        Vector2 min=lhs.min,max=lhs.max;
        return point.x>=min.x&&point.x<=max.x&&point.y>=min.y&&point.y<=max.y;
    }
}

public class Matrix3x3{
    public float m00,m01,m02;
    public float m10,m11,m12;
    public float m20,m21,m22;
    public Matrix3x3(){}
    public Matrix3x3(Vector3 col1, Vector3 col2, Vector3 col3){
        m00=col1.x;
        m10=col1.y;
        m20=col1.z;
        m01=col2.x;
        m11=col2.y;
        m21=col2.z;
        m02=col3.x;
        m12=col3.y;
        m22=col3.z;
    }
    public void SetColumn(int col, Vector3 val){
        switch(col){
            case 0:
                m00=val.x;
                m10=val.y;
                m20=val.z;
                break;
            case 1:
                m01=val.x;
                m11=val.y;
                m21=val.z;
                break;
            case 2:
                m02=val.x;
                m12=val.y;
                m22=val.z;
                break;
        }
    }
    public static Matrix3x3 operator*(Matrix3x3 lhs, Matrix3x3 rhs){
        Matrix3x3 res=new Matrix3x3();
        res.m00=lhs.m00*rhs.m00+lhs.m01*rhs.m10+lhs.m02*rhs.m20;
        res.m01=lhs.m00*rhs.m01+lhs.m01*rhs.m11+lhs.m02*rhs.m21;
        res.m02=lhs.m00*rhs.m02+lhs.m01*rhs.m12+lhs.m02*rhs.m22;
        res.m10=lhs.m10*rhs.m00+lhs.m11*rhs.m10+lhs.m12*rhs.m20;
        res.m11=lhs.m10*rhs.m01+lhs.m11*rhs.m11+lhs.m12*rhs.m21;
        res.m12=lhs.m10*rhs.m02+lhs.m11*rhs.m12+lhs.m12*rhs.m22;
        res.m20=lhs.m20*rhs.m00+lhs.m21*rhs.m10+lhs.m22*rhs.m20;
        res.m21=lhs.m20*rhs.m01+lhs.m21*rhs.m11+lhs.m22*rhs.m21;
        res.m22=lhs.m20*rhs.m02+lhs.m21*rhs.m12+lhs.m22*rhs.m22;
        return res;
    }
    public static Vector2 operator*(Matrix3x3 lhs, Vector3 rhs){
        return new Vector2(lhs.m00*rhs.x+lhs.m01*rhs.y+lhs.m02*rhs.z,lhs.m10*rhs.x+lhs.m11*rhs.y+lhs.m12*rhs.z);
    }
    public Vector2 Mul(Vector2 rhs, float rhsz){
        return new Vector2(m00*rhs.x+m01*rhs.y+m02*rhsz,m10*rhs.x+m11*rhs.y+m12*rhsz);
    }
}