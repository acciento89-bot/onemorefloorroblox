Shader "Floor/Surface"
{
 Properties {_Color("Tint",Color)=(1,1,1,1) _MainTex("Surface",2D)="white"{} _Glossiness("Polish",Range(0,1))=.3 _Metallic("Metal",Range(0,1))=0 _EmissionColor("Emission",Color)=(0,0,0,1)}
 SubShader {Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
 Pass {Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT
 #include "FloorUrpLighting.hlsl"
 sampler2D _MainTex;float4 _MainTex_ST;half4 _Color,_EmissionColor;half _Glossiness,_Metallic;
 struct V {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;half fogCoord:TEXCOORD3;};
 V vert(appdata_base v){V o;o.pos=TransformObjectToHClip(v.vertex.xyz);o.world=TransformObjectToWorld(v.vertex.xyz);o.normal=TransformObjectToWorldNormal(v.normal);o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);o.fogCoord=ComputeFogFactor(o.pos.z);return o;}
 half4 frag(V i):SV_Target{half3 n=normalize(i.normal);Light sun=GetMainLight(TransformWorldToShadowCoord(i.world));half3 albedo=tex2D(_MainTex,i.uv).rgb*_Color.rgb;half3 light=SampleSH(n)+sun.color*sun.distanceAttenuation*(.12+saturate(dot(n,sun.direction)))*sun.shadowAttenuation+FloorAdditionalDiffuse(i.world,n);half3 view=normalize(_WorldSpaceCameraPos-i.world);half spec=pow(saturate(dot(n,normalize(view+sun.direction))),lerp(12,80,_Glossiness));half3 colour=albedo*light+spec*sun.color*sun.distanceAttenuation*sun.shadowAttenuation*.18+_EmissionColor.rgb;return half4(MixFog(colour,i.fogCoord),1);}
 ENDHLSL }
 Pass {Name "DEPTHONLY" Tags {"LightMode"="DepthOnly"} ZWrite On ColorMask R HLSLPROGRAM
 #pragma vertex vertDepth
 #pragma fragment fragDepth
 #include "FloorUrpDepth.hlsl"
 ENDHLSL }
 Pass {Name "SHADOWCASTER" Tags {"LightMode"="ShadowCaster"} ZWrite On ZTest LEqual HLSLPROGRAM
 #pragma vertex vertShadow
 #pragma fragment fragShadow
 #include "FloorUrpShadow.hlsl"
 ENDHLSL }
 }
}
