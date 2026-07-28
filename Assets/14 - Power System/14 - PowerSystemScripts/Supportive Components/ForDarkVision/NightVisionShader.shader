// NightVisionShader.shader

Shader "Custom/NightVisionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 2)) = 1.0
        _Color ("Color", Color) = (0, 0.5, 0, 1)
        _Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
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
            float _Intensity;
            float4 _Color;
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
                
                // Converte para escala de cinza
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                
                // Aplica a cor verde da visão noturna
                float3 nightVision = gray * _Color.rgb * _Intensity;
                
                // Adiciona ruído
                float2 noiseUV = i.uv * 100;
                float noise = tex2D(_Noise, noiseUV).r;
                nightVision += noise * 0.1;
                
                // Vinheta (escurece as bordas)
                float2 center = i.uv - 0.5;
                float vignette = 1 - dot(center, center) * 2;
                vignette = max(0, vignette);
                nightVision *= vignette;
                
                return float4(nightVision, col.a);
            }
            ENDCG
        }
    }
}