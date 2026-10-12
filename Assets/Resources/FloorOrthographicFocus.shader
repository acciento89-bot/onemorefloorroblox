Shader "Floor/OrthographicFocus"
{
 SubShader {Tags {"RenderPipeline"="UniversalPipeline"} ZWrite Off ZTest Always Cull Off
 Pass {HLSLPROGRAM
 #pragma vertex Vert
 #pragma fragment Frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
 float4 _Focus;float _FocusFar;
 half4 Frag(Varyings i):SV_Target
 {
  float depth=SampleSceneDepth(i.texcoord);
  #if UNITY_REVERSED_Z
  depth=1-depth;
  #endif
  float distance=lerp(_Focus.w,_FocusFar,depth);
  float amount=saturate((distance-_Focus.x)/max(1,_Focus.y-_Focus.x));
  float2 radius=_BlitTexture_TexelSize.xy*_Focus.z*2.0*amount;
  half4 center=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord);
  if(amount<=.001)return center;
  half4 colour=center*.28;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord+float2(radius.x,0))*.12;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord-float2(radius.x,0))*.12;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord+float2(0,radius.y))*.12;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord-float2(0,radius.y))*.12;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord+radius)*.06;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord-radius)*.06;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord+float2(-radius.x,radius.y))*.06;
  colour+=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,i.texcoord+float2(radius.x,-radius.y))*.06;
  return colour;
 }
 ENDHLSL }
 }
}
