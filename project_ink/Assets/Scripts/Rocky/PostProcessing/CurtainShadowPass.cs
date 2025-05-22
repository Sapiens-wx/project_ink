using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CurtainShadowPass : ScriptableRenderPass
{
    static readonly string renderTag = "LineStyle Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("_TempTargetColorTint");
    private CurtainShadowEffect lineStyleVolume;
    private Material mat;
    RenderTargetIdentifier currentTarget; public CurtainShadowPass(RenderPassEvent passEvent, Shader lineStyleShader) { renderPassEvent = passEvent; if (lineStyleShader == null) { Debug.LogError("Shader不存在"); return; } mat = CoreUtils.CreateEngineMaterial(lineStyleShader); }
    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (mat==null||!renderingData.cameraData.postProcessEnabled||Trap_CurtainManager.inst==null)
            return;
        VolumeStack stack = VolumeManager.instance.stack;
        lineStyleVolume = stack.GetComponent<CurtainShadowEffect>();
        if (lineStyleVolume == null || !lineStyleVolume.IsActive())
            return;
        CommandBuffer cmd = CommandBufferPool.Get(renderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    private void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref CameraData cameraData = ref renderingData.cameraData;
        Camera camera = cameraData.camera;
        RenderTargetIdentifier source = currentTarget;
        int destination = TempTargetId;
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight, 0, FilterMode.Trilinear, RenderTextureFormat.Default);
        mat.SetTexture("_ShadowTex", Trap_CurtainManager.inst.rt_Shadow);
        mat.SetTexture("_MaskTex", Trap_CurtainManager.inst.rt_Mask);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, mat, 0);
    }
}