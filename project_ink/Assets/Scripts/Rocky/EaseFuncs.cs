using UnityEngine;

public class EaseFuncs{
    public static float EaseA1_2_yMovement(float time, float duration, float overshootOrAmplitude, float period){
        time/=duration;
        return Mathf.Sin(time*1.57f);
    }
}