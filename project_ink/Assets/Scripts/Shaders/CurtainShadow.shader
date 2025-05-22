Shader "Unlit/CurtainShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
		    Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ShadowTex, _MaskTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol=tex2D(_MainTex,i.uv);
                float b=tex2D(_ShadowTex,i.uv).a;
                b=step(.1,b);
                //mix with mask
                b*=tex2D(_MaskTex,i.uv).a;
                // sample the texture
                //fixed4 col = fixed4(b,b,b,1);
                fixed4 col = lerp(baseCol,fixed4(0,0,0,1),b);
                return col;
            }
            ENDCG
        }
    }
}
