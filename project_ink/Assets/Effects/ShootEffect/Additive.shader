Shader "Unlit/Additive"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        texSpd("tex spd",Float)=0
        _Mask ("Mask", 2D) = "White" {}
        maskSpd("mask spd",Float)=0
        [HDR]_Color ("color", Color) = (0,0,0,0)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 vertexColor : Color;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 vertexColor : Color;
            };

            sampler2D _MainTex,_Mask;
            float4 _MainTex_ST,_Mask_ST;
            fixed4 _Color;
            float maskSpd, texSpd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertexColor=v.vertexColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 texuv=i.uv;
                float2 maskuv=i.uv;
                texuv.y=frac(texuv.y+_Time.y*texSpd);
                maskuv.y=frac(maskuv.y+_Time.y*maskSpd);
                fixed4 col = tex2D(_MainTex, texuv)*i.vertexColor;
                col.a=col.r;
                col.a*=tex2D(_Mask, maskuv);
                col*=_Color;
                return col;
            }
            ENDCG
        }
    }
}
