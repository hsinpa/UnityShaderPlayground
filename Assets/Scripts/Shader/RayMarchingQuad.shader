Shader "Hsinpa/RayMarchingQuad"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RayDirection ("Ray Direction", Vector) = (0,0,0,0)
        _BallColor ("Ball Color", Color)  = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode "="ForwardBase"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST 1e-3

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _RayDirection;
            float4 _BallColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos,1));
                o.hitPos = v.vertex;

                return o;
            }

            float GetSphere(float3 p, float radius) {
                //Sphere
                return length(p) - radius;                
            }

            float RunDistantField(float3 p) {
                float displacement = sin(2.0 * p.x + _Time.x) * sin(5.0 * p.y + _Time.y) * sin(3.0 * p.z + _Time.x) * 0.25;

                float SphereDist_1 = GetSphere(p, 0.25);

                return SphereDist_1 + displacement;
            }

            float3 GetNormal(float3 p) {
                const float2 offset= float2(1e-2, 0.0);

                float3 n =RunDistantField(p) - float3(
                    RunDistantField(p - offset.xyy),
                    RunDistantField(p - offset.yxy),
                    RunDistantField(p - offset.yyx)
                );

                return normalize(n);
            }
 

            float Raymarch(float3 ro, float3 rd) {
                float dOrigin = 0;
                float dStep = 0;

                for (int i = 0; i < MAX_STEPS; i++) {
                    float3 p = ro + (dOrigin * rd);

                   dStep = RunDistantField(p);

                   dOrigin += dStep;
                    
                   if (dOrigin > MAX_DIST || dStep < SURF_DIST) break;
                }

                return dOrigin;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uv = i.uv - 0.5;
                float3 ro = i.ro;

                float3 rd = normalize(i.hitPos - ro); 
                //float3 rd = normalize(float3(uv.x, uv.y, 1));

                float d = Raymarch(ro, rd);

                float4 col = float4(0,0,0, 0);

                if (d < MAX_DIST) {
                   float3 position = ro + (d * rd);
                   float3 normal = GetNormal(position);

                   float light =  mul(normal, _WorldSpaceLightPos0.rgb);

                   col.rgb = _BallColor * light;
                   col.w = 1;
                }


                return col;
            }
            ENDCG
        }
    }
}
