// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// Make sure this file is not included twice
#ifndef GRASSLAYERS_INCLUDED
#define GRASSLAYERS_INCLUDED

// Include some helper functions
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGLayerGrassHelpers.hlsl"

struct Attributes {
    float3 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 uvAndHeight  : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
    float4 uvAndHeight  : TEXCOORD0; // (U, V, clipping noise height, color lerp)
    float3 positionWS   : TEXCOORD1; // Position in world space
    float3 normalWS     : TEXCOORD2; // Normal vector in world space

    float4 positionCS   : SV_POSITION; // Position in clip space
	
	UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO //Insert
};

// Properties
float4 _BaseColor;
float4 _TopColor;
// These two textures are combined to create the grass pattern in the fragment function
TEXTURE2D(_NoiseATexture); SAMPLER(sampler_NoiseATexture); float4 _NoiseATexture_ST;
float _NoiseABoost;
float _NoiseAScale;
TEXTURE2D(_NoiseBTexture); SAMPLER(sampler_NoiseBTexture); float4 _NoiseBTexture_ST;
float _NoiseBBoost;
float _NoiseBScale;
// Wind properties
TEXTURE2D(_WindTexture1); SAMPLER(sampler_WindTexture1); float4 _WindTexture1_ST;
TEXTURE2D(_WindTexture2); SAMPLER(sampler_WindTexture2); float4 _WindTexture2_ST;
float _WindFrequency;
float _WindAmplitude;

// Vertex function

VertexOutput Vertex(Attributes input) {
    // Initialize the output struct
    VertexOutput output = (VertexOutput)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    output.positionWS = GetVertexPositionInputs(input.positionOS).positionWS;
    output.normalWS = GetVertexNormalInputs(input.normalOS).normalWS;
    output.uvAndHeight = input.uvAndHeight;
    output.positionCS = TransformWorldToHClip(output.positionWS);

    return output;
}

// Fragment functions

half4 Fragment(VertexOutput input) : SV_Target {
    UNITY_SETUP_INSTANCE_ID(input);

    float2 uv = input.uvAndHeight.xy;
    float clipHeight = input.uvAndHeight.z;
	float layerHeight = input.uvAndHeight.w;

    // Calculate wind
    // Get the wind noise texture uv by applying scale and offset and then adding a time offset
    float2 windUV1 = TRANSFORM_TEX(uv, _WindTexture1) + _Time.y * _WindFrequency;
    float2 windUV2 = TRANSFORM_TEX(uv, _WindTexture2) + _Time.y * _WindFrequency;
    // Sample the wind noise texture and remap to range from -1 to 1
    float windNoise1 = SAMPLE_TEXTURE2D(_WindTexture1, sampler_WindTexture1, windUV1).x * 2 - 1;
    float windNoise2 = SAMPLE_TEXTURE2D(_WindTexture2, sampler_WindTexture2, windUV2).x * 2 - 1;

	float2 windNoise = float2(windNoise1, windNoise2);
	
    // Offset the grass UV by the wind. Higher layers are affected more
	uv += windNoise * (_WindAmplitude * layerHeight);

    // Sample the two noise textures, applying their scale and offset, and then the boost offset value
	float sampleA = SAMPLE_TEXTURE2D(_NoiseATexture, sampler_NoiseATexture, TRANSFORM_TEX(uv, _NoiseATexture)).r + _NoiseABoost;
	float sampleB = SAMPLE_TEXTURE2D(_NoiseBTexture, sampler_NoiseBTexture, TRANSFORM_TEX(uv, _NoiseBTexture)).r + _NoiseBBoost;
    // Combine the textures together using these scale variables. Lower values will reduce a texture's influence
	sampleA = 1 - (1 - sampleA) * _NoiseAScale;
	sampleB = 1 - (1 - sampleB) * _NoiseBScale;
    // If detailNoise * smoothNoise is less than height, this pixel will be discarded by the renderer
    // and will not render. The fragment function returns as well
	clip(sampleA * sampleB - clipHeight);

    // If the code reaches this far, this pixel should render

    // Gather some data for the lighting algorithm
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = NormalizeNormalPerPixel(input.normalWS); // Renormalize the normal to reduce interpolation errors
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS); // Calculate the view direction
    lightingInput.shadowCoord = CalculateShadowCoord(input.positionWS, input.positionCS); // Calculate the shadow map coord

    // Lerp between the three grass colors based on layer height
	float3 albedo = lerp(_BaseColor.rgb, _TopColor.rgb, layerHeight);

    // The URP simple lit algorithm
    // The arguments are lighting input data, albedo color, specular color, smoothness, emission color, and alpha

	SurfaceData surfaceData = (SurfaceData)0;
	surfaceData.albedo = lerp(_BaseColor.rgb, _TopColor.rgb, layerHeight);
	surfaceData.smoothness = 1;
	return UniversalFragmentBlinnPhong(lightingInput, surfaceData);
	
    //return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1);
}

#endif