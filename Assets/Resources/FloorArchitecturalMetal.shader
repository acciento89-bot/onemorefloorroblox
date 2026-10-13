Shader "Floor/ArchitecturalMetal" {
 Properties { _Color("Metal tint",Color)=(.74,.51,.23,1) _MainTex("Original brushed finish",2D)="white"{} _Glossiness("Polish",Range(0,1))=.68 _Metallic("Metal",Range(0,1))=.72 _EmissionColor("Emission",Color)=(0,0,0,1) _ReflectionCube("Original atrium reflection",Cube)="black"{} _ReflectionStrength("Scene reflection",Range(0,1))=0 }
 SubShader { Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"} LOD 150
 Pass { Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT
 #include "FloorUrpLighting.hlsl"
 sampler2D _MainTex;float4 _MainTex_ST;half4 _Color;half _Glossiness,_Metallic,_ReflectionStrength;TEXTURECUBE(_ReflectionCube);SAMPLER(sampler_ReflectionCube);
 struct metalInput {float4 vertex:POSITION;float3 normal:NORMAL;float2 texcoord:TEXCOORD0;half4 color:COLOR;};
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;UNITY_FOG_COORDS(3) SHADOW_COORDS(4) half4 tint:TEXCOORD5;};
 v2f vert(metalInput v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);o.tint=v.color;UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 half4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);
  half3 n=normalize(i.normal),view=GetWorldSpaceNormalizeViewDir(i.world),light=normalize(UnityWorldSpaceLightDir(i.world));half3 ray=reflect(-view,n);
  // Authored sunset/softbox reflection gives polished brass readable bands on mobile,
  // without a reflection camera, per-frame cubemap or a scene-global probe.
  half ceiling=smoothstep(-.24,.52,ray.y);
  half horizon=exp2(-abs(ray.y+.10)*14);
  half ribbon=pow(saturate(1-abs(ray.x*.82+ray.z*.36-.27)),64);
  half3 reflected=lerp(half3(.025,.060,.073),half3(.92,.73,.44),ceiling);
  reflected=lerp(reflected,half3(.12,.16,.17),horizon*.72);
  reflected+=ribbon*half3(1.25,1.18,.95);
  half3 room=SAMPLE_TEXTURECUBE_LOD(_ReflectionCube,sampler_ReflectionCube,ray,(1-_Glossiness)*6).rgb;
  reflected=lerp(reflected,room*1.25,_ReflectionStrength);
  half3 paint=lerp(half3(1,1,1),i.tint.rgb,step(.01,i.tint.a));half3 albedo=tex2D(_MainTex,i.uv).rgb*_Color.rgb*paint;
  half diffuse=saturate(dot(n,light));half spec=pow(saturate(dot(n,normalize(light+view))),lerp(20,96,_Glossiness));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.13,.13,.12));
  half fresnel=pow(1-saturate(dot(n,view)),4);
  half3 result=albedo*(ambient*.32+_LightColor0.rgb*diffuse*atten*.44)+reflected*lerp(albedo,half3(.91,.79,.50),.22)*(.25+_Metallic*.70)+_LightColor0.rgb*spec*atten*2.4+fresnel*half3(.18,.15,.09);
  result+=albedo*FloorAdditionalDiffuse(i.world,n);result=result/(1+result*.38);half4 col=half4(result,1);UNITY_APPLY_FOG(i.fogCoord,col);return col;
 }
 ENDHLSL }
 Pass {Name "DEPTHONLY" Tags {"LightMode"="DepthOnly"} ZWrite On ColorMask R HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vertDepth
 #pragma fragment fragDepth
 #include "FloorUrpDepth.hlsl"
 ENDHLSL }
 Pass {Name "SHADOWCASTER" Tags {"LightMode"="ShadowCaster"} ZWrite On ZTest LEqual
 HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vertShadow
 #pragma fragment fragShadow
 #include "FloorUrpShadow.hlsl"
 ENDHLSL }
 }
}
