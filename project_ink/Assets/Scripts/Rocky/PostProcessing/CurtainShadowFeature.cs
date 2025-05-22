using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CurtainShadowFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();
    CurtainShadowPass pass;
    public override void Create()
    {
        this.name = "CurtainShadowPass";
        pass = new CurtainShadowPass(RenderPassEvent.BeforeRenderingPostProcessing, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        base.SetupRenderPasses(renderer, renderingData);
        pass.Setup(renderer.cameraColorTargetHandle);
    }
}