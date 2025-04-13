Shader "Unlit/Tentacle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        tex_blend("texture blend", Float) = 0
        tex_contrast("texture contrast", Float) = 0
        _alpha("alpha", Float)=0
        colour_1("color_1", Color)=(1,1,1,1)
        colour_2("color_2", Color)=(1,1,1,1)
        colour_3("color_3", Color)=(1,1,1,1)
        col_power("color power", Float)=0
        power("power fresnel", Float)=0
        coef("coef fresnel", Float)=0
    }
    SubShader
    {
        LOD 100
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            float tex_blend;
            float _alpha;
            float col_power;
            float4 colour_1;
            float4 colour_2;
            float4 colour_3;
            float power, coef;
            float tex_contrast;

            #define SPIN_EASE 0.5
            #define PI 3.1415926535898

            float easeInOutCubic(float x){
                return x<.5?pow(x/4,.333):1-pow(2-2*x,.333)/2;
            }
            float pushToEdge(float x, float strength) {
                if (x < 0.5) {
                    return 0.5 * pow(2.0 * x, strength);
                } else {
                    return 1.0 - 0.5 * pow(2.0 * (1.0 - x), strength);
                }
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float b;
                b=pow(sin(PI*i.uv.y),col_power);
                b=b*step(i.uv.y,.5)+step(.5,i.uv.y)*(2-b);
                b*=0.5;
                b=saturate(b);
                float fresnel=1+pow(smoothstep(0,.5,abs(i.uv.y-0.5)),power)*coef;
                // weights
                float weight1 = 1.0 - smoothstep(0.0, 0.5, b); // 0.0 时最大，0.5 时最小
                float weight2 = 1.0 - abs(b - 0.5) * 2.0;     // 0.5 时最大，0.0 和 1.0 时最小
                float weight3 = smoothstep(0.5, 1.0, b);       // 1.0 时最大，0.5 时最小
                // texture
                float texB=tex2D(_MainTex,i.uv).x*tex_contrast;

                // 计算插值后的颜色
                fixed4 col=colour_1*weight1+colour_2*weight2+colour_3*weight3;
                col.xyz*=fresnel;
                col*=lerp(fixed4(1,1,1,1),fixed4(texB,texB,texB,1),tex_blend);
                col.a=_alpha;
                return col;
            }
            ENDCG
        }
    }
}
