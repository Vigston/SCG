Shader "Custom/EdgeFadeLocal"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _FadeStart ("Fade Start", Range(0,1)) = 0.4
        _FadeEnd ("Fade End", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float3 localPos : TEXCOORD1; // ローカル座標をそのまま渡す
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
                o.localPos = v.vertex.xyz; // これで安定したローカル座標
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ローカル座標を使用（カメラに依存しない）
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