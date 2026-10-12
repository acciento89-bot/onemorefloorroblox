#ifndef FLOOR_URP_LIGHTING
#define FLOOR_URP_LIGHTING
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
// Preserve the original authored surface equations while using real URP lights.
struct appdata_base {float4 vertex:POSITION;float3 normal:NORMAL;float2 texcoord:TEXCOORD0;};
struct appdata_full {float4 vertex:POSITION;float3 normal:NORMAL;float4 texcoord:TEXCOORD0;half4 color:COLOR;};
#define UnityObjectToClipPos(v) TransformObjectToHClip((v).xyz)
#define UnityObjectToWorldNormal(n) TransformObjectToWorldNormal(n)
#define UnityWorldSpaceLightDir(p) GetMainLight().direction
#define ShadeSH9(n) SampleSH((n).xyz)
#define _LightColor0 (half4(GetMainLight().color*GetMainLight().distanceAttenuation,1))
#define UNITY_FOG_COORDS(idx) half fogCoord:TEXCOORD##idx;
#define UNITY_TRANSFER_FOG(o,p) o.fogCoord=ComputeFogFactor((p).z)
#define UNITY_APPLY_FOG(f,c) c.rgb=MixFog(c.rgb,f)
#define SHADOW_COORDS(idx)
#define TRANSFER_SHADOW(o)
#define UNITY_LIGHT_ATTENUATION(a,i,w) half a=GetMainLight(TransformWorldToShadowCoord(w)).shadowAttenuation
half3 FloorAdditionalDiffuse(float3 world,half3 normal)
{
 half3 fill=0;
 #if defined(_ADDITIONAL_LIGHTS)
 uint count=GetAdditionalLightsCount();
 for(uint lightIndex=0u;lightIndex<count;lightIndex++)
 {
  Light lamp=GetAdditionalLight(lightIndex,world);
  fill+=lamp.color*lamp.distanceAttenuation*lamp.shadowAttenuation*saturate(dot(normal,lamp.direction));
 }
 #endif
 return fill;
}
#endif
