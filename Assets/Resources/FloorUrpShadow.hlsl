#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
float3 _LightDirection;
struct FloorShadowInput {float4 vertex:POSITION;float3 normal:NORMAL;};
struct FloorShadowOutput {float4 pos:SV_POSITION;};
FloorShadowOutput vertShadow(FloorShadowInput v)
{
 FloorShadowOutput o;float3 world=TransformObjectToWorld(v.vertex.xyz);
 o.pos=TransformWorldToHClip(ApplyShadowBias(world,TransformObjectToWorldNormal(v.normal),_LightDirection));
 #if UNITY_REVERSED_Z
 o.pos.z=min(o.pos.z,UNITY_NEAR_CLIP_VALUE);
 #else
 o.pos.z=max(o.pos.z,UNITY_NEAR_CLIP_VALUE);
 #endif
 return o;
}
half4 fragShadow(FloorShadowOutput i):SV_Target{return 0;}
