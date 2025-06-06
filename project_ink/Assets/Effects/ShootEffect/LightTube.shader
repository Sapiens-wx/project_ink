Shader "Unlit/LightTube"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        baseColor ("base color", Color) = (0,0,0,0)
        hlColor ("highlight color", Color) = (0,0,0,0)
        _alpha ("alpha", Float) = 0
    }
    SubShader
    {
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #define PI 3.14159265358979

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
            fixed4 baseColor,hlColor;
            float _alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float b=sin(PI*i.uv.y);
    
                fixed4 color = baseColor*b+(1-b)*hlColor;
                color.a=pow(lerp(1,_alpha,b),3);
                return color;
            }
            ENDCG
        }
    }
}
