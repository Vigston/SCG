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
        // �A�E�g���C���͑����O�ɕ`�悵�āA�������ɂ�������Ȃ��悤��
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        // ���ʂ����`��i�w�ʖ@�j
        Cull Front
        // Z�o�b�t�@�Ɋ����Ȃ��悤��
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

                // �@�������[���h��Ԃɕϊ�
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldOffset = normalize(worldNormal) * _OutlineWidth;

                // �I�u�W�F�N�g��Ԃɖ߂��Ē��_�ɉ��Z
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