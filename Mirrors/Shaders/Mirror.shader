Shader "stoogebag/Mirror"
{
    Properties
    {
        _MainTex ("Mirror Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Mirror"

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 screenPos : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _Tint;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.screenPos = ComputeScreenPos(o.vertex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.screenPos.xy / i.screenPos.w;
                    uv.x = 1.0 - uv.x;
                    float4 col = tex2D(_MainTex, uv);
                    return col * _Tint;
                }
            ENDHLSL
        }
    }

    Fallback Off
}
