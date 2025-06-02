using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("Custom/CurtainShaddowEffect")]
public class CurtainShadowEffect : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => Trap_CurtainManager.inst!=null && Trap_CurtainManager.inst.isActive;
    public bool IsTileCompatible() => false;
}