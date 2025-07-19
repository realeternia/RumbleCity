Shader "Custom/UnlitTextureWithSpecular_BuiltIn" 
{
    Properties 
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        [HDR] _CustomSpecColor ("Specular Color", Color) = (1,1,1,1)
        _Glossiness ("Shininess", Range(1, 256)) = 32
        _SpecIntensity ("Specular Intensity", Range(0, 2)) = 1
    }
    
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            
            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f 
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };
            
            // 使用唯一变量名声明
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _CustomSpecColor;
            half _Glossiness;
            half _SpecIntensity;
            
            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target 
            {
                // 标准化向量
                float3 N = normalize(i.worldNormal);
                float3 V = normalize(i.viewDir);
                float3 L = _WorldSpaceLightPos0.xyz;
                
                // 采样纹理
                fixed4 texCol = tex2D(_MainTex, i.uv);
                fixed4 col = texCol * _Color;
                
                // 漫反射计算
                float diff = max(0, dot(N, L));
                float3 diffuse = _LightColor0.rgb * diff;
                
                // 高光计算 (Blinn-Phong)
                float3 H = normalize(L + V);
                float spec = pow(max(0, dot(N, H)), _Glossiness);
                float3 specular = _CustomSpecColor.rgb * spec * _SpecIntensity * _LightColor0.rgb;
                
                // 组合最终颜色
                float3 finalColor = col.rgb * (diffuse + specular);
                
                // 应用雾效
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                
                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}