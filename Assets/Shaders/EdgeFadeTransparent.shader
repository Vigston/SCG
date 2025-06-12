// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/EdgeFade_OutlineAlwaysFront"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _FadeStart ("Fade Start", Range(0,1)) = 0.4
        _FadeEnd ("Fade End", Range(0,1)) = 0.5
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // --- Pass 1: アウトライン（最前面に描画） ---
        Cull Front
        ZWrite Off
        ZTest Always // ★ 常にカメラ前面に表示！
        Pass
        {
            Name "OUTLINE"
            CGPROGRAM
            #pragma vertex vert_outline
            #pragma fragment frag_outline
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vert_outline (appdata v)
            {
                v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 offset = normalize(worldNormal) * _OutlineWidth;
                v.vertex.xyz += mul((float3x3)unity_WorldToObject, offset);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag_outline (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // --- Pass 2: 本体（フェード・通常の奥行き順） ---
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual
        Pass
        {
            Name "FADE_BODY"
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeStart;
            float _FadeEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fadeX = saturate((_FadeEnd - abs(i.localPos.x)) / (_FadeEnd - _FadeStart));
                float fadeZ = saturate((_FadeEnd - abs(i.localPos.z)) / (_FadeEnd - _FadeStart));
                float fade = fadeX * fadeZ;

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= fade;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}