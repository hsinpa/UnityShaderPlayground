Shader "Hsinpa/Outline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (.0, .0, .0, 1)
        _OutlineWidth("Outline Width",  Range(0.0, 0.01)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float3 N = mul((float3x3) UNITY_MATRIX_MV, v.normal);
                float2 offset = mul((float2x2) UNITY_MATRIX_P, N.xy);

                o.vertex.xy += offset * _OutlineWidth;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
