// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/BackfaceOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 1, 1, 1)
        _OutlineWidth ("Outline Width", Float) = 0.03
    }
    SubShader
    {
        // アウトラインは他より前に描画して、透明物にも埋もれないように
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        // 裏面だけ描画（背面法）
        Cull Front
        // Zバッファに干渉しないように
        ZWrite Off
        ZTest Less

        Pass
        {
            Name "OUTLINE"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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

            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;

                // 法線をワールド空間に変換
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldOffset = normalize(worldNormal) * _OutlineWidth;

                // オブジェクト空間に戻して頂点に加算
                v.vertex.xyz += mul((float3x3)unity_WorldToObject, worldOffset);

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}