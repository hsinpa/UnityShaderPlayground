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
                float t = clamp(dot(pa, ba) / (dot(ba, ba) + 0.0001), 0.0, 1.0);

                return length(pa - ba * t);
            }

            float Line(float2 p, float2 a, float2 b) {
                float d = DistLine(p, a, b);
                float m = smoothstep(0.03, 0.01, d);
                m *= smoothstep(1.2, 0.8, length(a-b));
                return m;
            }

            float Hash21(float2 p) {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            float2 Hash22(float2 p) {
                float n = Hash21(p);
                return float2(n, Hash21(p + n));
            }

            float2 GetPos(float2 id, float2 offset) {
                float2 n = Hash22(id + offset) * _Time.y;
                return offset + sin(n) * 0.3;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.uv - 0.5);
                float m = 0;

                uv *=5;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);

                float2 p[9];
                int index = 0;
                for (float y = -1.0; y <= 1.0; y++) {
                    for (float x = -1.0; x <= 1.0; x++) {
                        p[index++] = GetPos(id, float2(x,y));
                    }
                }

                for (int t = 0; t < 9; t++) {
                    m += Line(gv, p[4], p[t]);

                    float2 j = (p[t] - gv) * 15.0;
                    float sparkle = 1. / dot(j,j);
                    m += sparkle * (sin(t + p[t].x * 10.0) * 0.5 + 0.5);

                }
                m += Line(gv, p[1], p[3]);
                m += Line(gv, p[1], p[5]);
                m += Line(gv, p[7], p[3]);
                m += Line(gv, p[7], p[5]);


                float4 col = float4(m,m,m, 1);

                //if (gv.x > 0.48 || gv.y > 0.48) col = float4(1,0,0,1);

                return col;
            }
            ENDCG
        }
    }
}
