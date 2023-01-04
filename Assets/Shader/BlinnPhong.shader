Shader "Hsinpa/BlinnPhong"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (.25, .5, .5, 1)
        _Specular("Specular", Range(0, 1)) = 0.5
        _Gloss("Glossy", Float) = 5.0
        _Fresnel("Fresnel", Float) = 1.0
        _FresnelColor("Fresnel Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Specular;
            float _Gloss;

            float _Fresnel;
            float4 _FresnelColor;

            half StepFeatherToon(half Term, half maxTerm, half step, half feather)
            {
                return saturate((Term / maxTerm - step) / feather) * maxTerm;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 ambient = unity_AmbientSky;
                fixed4 direction_light_color = fixed4(_LightColor0.xyz, 1.0);
                float4 light_direction = normalize(_WorldSpaceLightPos0);
 
                float light_strength = smoothstep(0.1, 0.5,  dot(light_direction, i.worldNormal) );

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                //Specular
                fixed3 reflect_direction = reflect(-light_direction, i.worldNormal);
                fixed3 view_direciton = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                fixed specularStrength = smoothstep(0.2, 0.3, _Specular * pow(saturate(dot(reflect_direction, view_direciton)), _Gloss));

                fixed3 specularColor = direction_light_color.xyz * specularStrength;

                //Fresnel
                float fresnelStrength = pow(1.0 - saturate(dot( i.worldNormal, view_direciton)) , _Fresnel);

                fixed4 output = (col * direction_light_color * light_strength) + fixed4(specularColor, 1.0);
                output.xyz += ambient.xyz;

                output.xyz = lerp(output.xyz, _FresnelColor, fresnelStrength);


                return output;
            }
            ENDCG
        }
    }
} 
