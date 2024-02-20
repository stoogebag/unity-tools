Shader "Grass/LayerGrass"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0,0.5,0,1)
        [HDR]_TopColor("Top Color", Color) = (0,1,0,1)
        
        _NoiseATexture("Noise A", 2D) = "white" {}
        _NoiseABoost("Noise A offset", Range(-1,1)) = 0
        _NoiseAScale("Noise A scale", Range(0,3)) = 1
        
        _NoiseBTexture("Noise B", 2D) = "white" {}
        _NoiseBBoost("Noise B offset", Range(-1,1)) = 0
        _NoiseBScale("Noise B scale", Range(0,3)) = 1
        
        _WindTexture1("Wind Noise 1", 2D) = "white" {}
        _WindTexture2("Wind Noise 2", 2D) = "white" {}
        _WindFrequency("wind freq", float) = 1
        _WindAmplitude("wind strength", float) = 1
        
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            
            HLSLPROGRAM
            #pragma  prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS 
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma  vertex Vertex
            #pragma fragment Fragment

            #include "LayerGrass.hlsl"

            
            
            
            ENDHLSL
        }
    }
}
