Shader "Custom/XRayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0, 1, 0, 0.3)
        _Intensity ("Intensity", Range(0, 2)) = 1.0
        _Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            sampler2D _Noise;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                // Efeito raio-x (tons de verde)
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float4 xray = float4(gray * _Color.rgb * _Intensity, _Color.a);
                
                // Adiciona ruído para efeito realista
                float noise = tex2D(_Noise, i.uv * 10).r;
                xray.rgb += noise * 0.05;
                
                return xray;
            }
            ENDCG
        }
    }
}