Shader "Floor/RunnerSurface" {
 Properties {_Color("Garment or skin tint",Color)=(1,1,1,1) _MainTex("Owned strand or cloth finish",2D)="white"{} _Glossiness("Surface polish",Range(0,1))=.2 _SurfaceKind("Cloth / skin / hair",Float)=0}
 SubShader {Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"} LOD 150
 Pass {Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT
 #pragma multi_compile_fog
 #include "FloorUrpLighting.hlsl"
 sampler2D _MainTex;float4 _MainTex_ST;half4 _Color;half _Glossiness,_SurfaceKind;
 struct v2f{float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;half4 tint:COLOR;UNITY_FOG_COORDS(3) SHADOW_COORDS(4)};
 v2f vert(appdata_full v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);half overrideTint=step(.49,v.color.a)*(1-step(.51,v.color.a));o.tint=half4(lerp(_Color.rgb*v.color.rgb,v.color.rgb,overrideTint),1);UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 half4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);half3 n=normalize(i.normal),view=normalize(_WorldSpaceCameraPos.xyz-i.world),light=normalize(UnityWorldSpaceLightDir(i.world));half ndl=dot(n,light);
  // Accent flags are resolved per vertex before interpolation; mixed cloth/pack
  // triangles interpolate actual colour, never a thresholded half-alpha flag.
  half3 albedo=i.tint.rgb*tex2D(_MainTex,i.uv).rgb;
  half skin=1-step(.1,abs(_SurfaceKind-1)),hair=step(1.5,_SurfaceKind);half wrap=lerp(.12,.35,skin);half diffuse=saturate((ndl+wrap)/(1+wrap));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.22,.23,.22));half spec=pow(saturate(dot(n,normalize(light+view))),lerp(18,70,_Glossiness));half edge=pow(1-saturate(dot(n,view)),3);
  half3 colour=albedo*(ambient*.78+_LightColor0.rgb*(.10+diffuse*.80)*lerp(.48,1,atten));
  colour+=albedo*FloorAdditionalDiffuse(i.world,n);colour+=_LightColor0.rgb*spec*atten*lerp(.08,.23,skin+hair);colour+=albedo*edge*lerp(.07,.17,hair);colour+=skin*half3(.035,.016,.008)*saturate(1-ndl)*atten;
  half4 c=half4(colour,1);UNITY_APPLY_FOG(i.fogCoord,c);return c;
 } ENDHLSL }
 Pass {Name "DEPTHONLY" Tags {"LightMode"="DepthOnly"} ZWrite On ColorMask R HLSLPROGRAM
 #pragma vertex vertDepth
 #pragma fragment fragDepth
 #include "FloorUrpDepth.hlsl"
 ENDHLSL }
 Pass {Name "SHADOWCASTER" Tags {"LightMode"="ShadowCaster"} ZWrite On ZTest LEqual
 HLSLPROGRAM
 #pragma vertex vertShadow
 #pragma fragment fragShadow
 #include "FloorUrpShadow.hlsl"
 ENDHLSL }
 }
}
