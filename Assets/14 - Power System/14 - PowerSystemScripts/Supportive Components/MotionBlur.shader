// MotionBlur.shader (crie como um arquivo .shader)
Shader "Custom/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1
        _Blend ("Blend", Float) = 1
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
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _Blend;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.uv);
                float4 blur = tex2D(_MainTex, i.uv + float2(_BlurSize * _MainTex_TexelSize.x, 0));
                blur += tex2D(_MainTex, i.uv - float2(_BlurSize * _MainTex_TexelSize.x, 0));
                blur += tex2D(_MainTex, i.uv + float2(0, _BlurSize * _MainTex_TexelSize.y));
                blur += tex2D(_MainTex, i.uv - float2(0, _BlurSize * _MainTex_TexelSize.y));
                blur *= 0.25;
                
                return lerp(color, blur, _Blend);
            }
            ENDCG
        }
    }
}