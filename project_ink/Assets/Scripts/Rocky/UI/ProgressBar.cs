using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image bar;
    
    private float progress;
    public void SetProgress(float time){
        progress=time;
        bar.fillAmount=progress;
    }
}
