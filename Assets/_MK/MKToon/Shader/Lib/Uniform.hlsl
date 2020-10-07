//////////////////////////////////////////////////////
// MK Toon Uniform					       			//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

#ifndef MK_TOON_UNIFORM
	#define MK_TOON_UNIFORM

	#if defined(MK_URP)
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#elif defined(MK_LWRP)
		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
	#else
		#include "UnityCG.cginc"
	#endif

	#include "Pipeline.hlsl"

	/////////////////////////////////////////////////////////////////////////////////////////////
	// UNIFORM VARIABLES
	/////////////////////////////////////////////////////////////////////////////////////////////
	// The compiler should optimized the code by stripping away unused uniforms
	// This way its also possible to avoid an inconsistent buffer size error and
	// use the SRP Batcher, while compile different variants of the shader
	// Every PerDraw (builtin engine variables) should accessed via the builtin include files
	// Its not clear if a block based setup for the srp batcher is required,
	// therefore all uniforms are grouped this way:
	//
	// fixed 	 | fixed2 	 | fixed3 	 | fixed4
	// half  	 | half2  	 | half3  	 | half4
	// float 	 | float2 	 | float3 	 | float4
	// Sampler2D | Sampler3D
	
	CBUFFER_START(UnityPerMaterial)
		uniform autoLP _AlphaCutoff;
		uniform autoLP _Metallic;
		uniform autoLP _Smoothness;
		uniform autoLP _Roughness;
		uniform autoLP _Anisotropy;
		uniform autoLP _LightTransmissionDistortion;
		uniform autoLP _LightBandsScale;
		uniform autoLP _LightThreshold;
		uniform autoLP _DrawnClampMin;
		uniform autoLP _DrawnClampMax;
		uniform autoLP _Contrast;
		uniform autoLP _Saturation;
		uniform autoLP _Brightness;
		uniform autoLP _DiffuseSmoothness;
		uniform autoLP _DiffuseThresholdOffset;
		uniform autoLP _SpecularSmoothness;
		uniform autoLP _SpecularThresholdOffset;
		uniform autoLP _RimSmoothness;
		uniform autoLP _RimThresholdOffset;
		uniform autoLP _IridescenceSmoothness;
		uniform autoLP _IridescenceThresholdOffset;
		uniform autoLP _LightTransmissionSmoothness;
		uniform autoLP _LightTransmissionThresholdOffset;
		uniform autoLP _RimSize;
		uniform autoLP _IridescenceSize;
		uniform autoLP _DissolveAmount;
		uniform autoLP _DissolveBorderSize;
		uniform autoLP _OutlineNoise;
		uniform autoLP _DiffuseWrap;
		uniform autoLP _DetailMix;
		uniform autoLP _RefractionDistortionFade;
		uniform autoLP _GoochRampIntensity;
		uniform autoLP _VertexAnimationIntensity;

		uniform autoLP3 _DetailColor;
		uniform autoLP3 _SpecularColor;
		uniform autoLP3 _LightTransmissionColor;

		uniform autoLP4 _AlbedoColor;
		uniform autoLP4 _DissolveBorderColor;
		uniform autoLP4 _OutlineColor;
		uniform autoLP4 _IridescenceColor;
		uniform autoLP4 _RimColor;
		uniform autoLP4 _RimBrightColor;
		uniform autoLP4 _RimDarkColor;
		uniform autoLP4 _GoochDarkColor;
		uniform autoLP4 _GoochBrightColor;
		uniform autoLP4 _VertexAnimationFrequency;

		uniform half _DetailNormalMapIntensity;
		uniform half _NormalMapIntensity;
		uniform half _Parallax;
		uniform half _OcclusionMapIntensity;
		uniform half _LightBands;
		uniform half _ThresholdMapScale;
		uniform half _ArtisticFrequency;
		uniform half _DissolveMapScale;
		uniform half _DrawnMapScale;
		uniform half _SketchMapScale;
		uniform half _HatchingMapScale;
		uniform half _OutlineSize;
		uniform half _SpecularIntensity;
		uniform half _LightTransmissionIntensity;
		uniform half _RefractionDistortionMapScale;
		uniform half _IndexOfRefraction;
		uniform half _RefractionDistortion;

		uniform half3 _EmissionColor;

		uniform float _SoftFadeNearDistance;
		uniform float _SoftFadeFarDistance;
		uniform float _CameraFadeNearDistance;
		uniform float _CameraFadeFarDistance;
		
		uniform float4 _AlbedoMap_ST;
		uniform float4 _MainTex_ST;
		uniform float4 _DetailMap_ST;
	CBUFFER_END

	UNIFORM_TEXTURE_2D(_AlbedoMap);
	UNIFORM_TEXTURE_2D(_RefractionDistortionMap);
	UNIFORM_TEXTURE_2D(_SpecularMap);
	UNIFORM_TEXTURE_2D(_RoughnessMap);
	UNIFORM_TEXTURE_2D(_MetallicMap);
	UNIFORM_TEXTURE_2D(_DetailMap);
	UNIFORM_TEXTURE_2D(_DetailNormalMap);
	UNIFORM_TEXTURE_2D(_NormalMap);
	UNIFORM_TEXTURE_2D(_HeightMap);
	UNIFORM_TEXTURE_2D(_ThicknessMap);
	UNIFORM_TEXTURE_2D(_OcclusionMap);
	UNIFORM_TEXTURE_2D(_ThresholdMap);
	UNIFORM_TEXTURE_2D(_GoochRamp);
	UNIFORM_TEXTURE_2D(_DiffuseRamp);
	UNIFORM_TEXTURE_2D(_SpecularRamp);
	UNIFORM_TEXTURE_2D(_RimRamp);
	UNIFORM_TEXTURE_2D(_LightTransmissionRamp);
	UNIFORM_TEXTURE_2D(_IridescenceRamp);
	UNIFORM_TEXTURE_2D(_DrawnMap);
	UNIFORM_TEXTURE_2D(_HatchingBrightMap);
	UNIFORM_TEXTURE_2D(_HatchingDarkMap);
	UNIFORM_TEXTURE_2D(_GoochBrightMap);
	UNIFORM_TEXTURE_2D(_GoochDarkMap);
	UNIFORM_TEXTURE_2D(_SketchMap);
	UNIFORM_TEXTURE_2D(_DissolveMap);
	UNIFORM_TEXTURE_2D(_DissolveBorderRamp);
	UNIFORM_TEXTURE_2D(_EmissionMap);
	uniform sampler2D _VertexAnimationMap;
	uniform sampler3D _DitherMaskLOD;
#endif