Shader "Hsinpa/Outline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (.0, .0, .0, 1)
        _OutlineWidth("Outline Width",  Range(0.0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        pass
        {
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            half _OutlineWidth;
            half4 _OutLineColor;

            struct a2v 
	    {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertColor : COLOR;
                float4 tangent : TANGENT;
            };

            struct v2f
	   {
                float4 pos : SV_POSITION;
            };


            v2f vert (a2v v) 
	       {
                v2f o;
		        UNITY_INITIALIZE_OUTPUT(v2f, o);

                float4 pos = UnityObjectToClipPos(v.vertex);
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.tangent.xyz);

                float3 ndcNormal = normalize(TransformViewToProjection(viewNormal.xyz)) * pos.w;//将法线变换到NDC空间
                float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));//将近裁剪面右上角位置的顶点变换到观察空间
                float aspect = abs(nearUpperRight.y / nearUpperRight.x);//求得屏幕宽高比

                ndcNormal.x *= aspect;
                pos.xy += 0.01 * _OutlineWidth * ndcNormal.xy;
                o.pos = pos;

                return o;
            }

            half4 frag(v2f i) : SV_TARGET 
	    {
                return _OutLineColor;
            }
            ENDCG
        }
    }
}
