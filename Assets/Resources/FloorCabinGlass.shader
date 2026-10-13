Shader "Floor/CabinGlass" {
 Properties {[HideInInspector] _MainTex("Compatibility white texture",2D)="white"{} _Color("Clear teal glass",Color)=(.35,.64,.64,.09) _Glossiness("Polish",Range(0,1))=.8 _Metallic("Metal",Range(0,1))=0 _ReflectionCube("Original atrium reflection",Cube)="black"{} _ReflectionStrength("Scene reflection",Range(0,1))=0}
 SubShader {Tags {"Queue"="Transparent+10" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline"} LOD 100 Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
 Pass {Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #include "FloorUrpLighting.hlsl"
 half4 _Color;half _Glossiness,_ReflectionStrength;TEXTURECUBE(_ReflectionCube);SAMPLER(sampler_ReflectionCube);
 struct v2f{float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;UNITY_FOG_COORDS(2)};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);return o;}
 half4 frag(v2f i):SV_Target{half3 n=normalize(i.normal),view=GetWorldSpaceNormalizeViewDir(i.world);half fresnel=pow(1-abs(dot(n,view)),4);half3 reflected=reflect(-view,n);half ribbon=pow(saturate(1-abs(reflected.x*.7+reflected.z*.3-.23)),34);half3 room=SAMPLE_TEXTURECUBE_LOD(_ReflectionCube,sampler_ReflectionCube,reflected,(1-_Glossiness)*6).rgb;half4 col=half4(_Color.rgb+half3(.23,.22,.15)*ribbon+room*_ReflectionStrength*(.3+fresnel),_Color.a+fresnel*.12+ribbon*.055);UNITY_APPLY_FOG(i.fogCoord,col);return col;}
 ENDHLSL } }
}
