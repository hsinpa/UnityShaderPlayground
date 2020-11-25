Shader "Unlit/TruchetTilingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float DistLine(float2 p, float2 a, float2 b) {
                float2 pa = p - a;
                float2 ba = b - a;
                float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);

                return length(pa - ba * t);
            }

            float Hash21(float2 p) {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.uv);

                uv *=5;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);

                float randomNumber = Hash21(uv);
                float4 col = float4(0, 0, 0, 1);

                float lineWidth = 0.1;
                float mask = smoothstep(0.01, -0.01, abs(gv.x + gv.y) - lineWidth);

                col += mask;

                if (gv.x > 0.48 || gv.y > 0.48) col = float4(1,0,0,1);

                return col;
            }
            ENDCG
        }
    }
}
