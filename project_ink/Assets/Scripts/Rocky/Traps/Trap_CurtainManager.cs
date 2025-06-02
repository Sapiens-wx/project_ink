using UnityEngine;
using System;

public class Trap_CurtainManager : Singleton<Trap_CurtainManager>{
    public Camera camShadow,camMask;

    [HideInInspector][NonSerialized] public bool isActive;
    [HideInInspector][NonSerialized] public RenderTexture rt_Shadow, rt_Mask;
    void Start(){
        rt_Shadow=new RenderTexture(camShadow.scaledPixelWidth, camShadow.scaledPixelHeight, 16);
        rt_Shadow.Create();
        rt_Mask=new RenderTexture(camShadow.scaledPixelWidth, camShadow.scaledPixelHeight, 16);
        rt_Mask.Create();
        camShadow.targetTexture=rt_Shadow;
        camMask.targetTexture=rt_Mask;
        SetActive(true);
    }
    public void SetActive(bool val){
        camShadow.gameObject.SetActive(val);
        camMask.gameObject.SetActive(val);
        isActive=val;
    }
    void OnDestroy(){
        rt_Shadow.Release();
        rt_Mask.Release();
    }
}