Shader "Floor/Foliage" {
 Properties {_MainTex("Original ivy finish",2D)="white"{} _Color("Leaf tint",Color)=(.18,.34,.10,1)}
 SubShader {Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"} LOD 150 Cull Off
 Pass {Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT
 #include "FloorUrpLighting.hlsl"
 sampler2D _MainTex;half4 _Color;
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;half4 tint:COLOR;UNITY_FOG_COORDS(3) SHADOW_COORDS(4)};
 v2f vert(appdata_full v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);o.uv=v.texcoord.xy;o.tint=v.color;UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 half4 frag(v2f i,half facing:VFACE):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);half3 n=normalize(i.normal)*(facing>=0?1:-1),light=normalize(UnityWorldSpaceLightDir(i.world));half diffuse=saturate(dot(n,light));
  float mid=abs(i.uv.x-.5),branch=abs(frac(i.uv.y*5.0-mid*3.8)-.5);half vein=(1-smoothstep(.006,.023,mid))*.25+(1-smoothstep(.012,.035,branch))*.09;
  half edge=smoothstep(.17,.49,mid);half3 tint=_Color.rgb*i.tint.rgb*(.80+i.uv.y*.28-edge*.12)+half3(.20,.26,.07)*vein;
  half3 atlas=tex2D(_MainTex,float2(.505+saturate(i.uv.x)*.49,.005+saturate(i.uv.y)*.99)).rgb;tint*=lerp(half3(1,1,1),atlas*2.4,.32);
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.19,.22,.14));half back=pow(saturate(dot(-n,light)),2)*.23;half rim=pow(1-saturate(dot(n,normalize(_WorldSpaceCameraPos.xyz-i.world))),3)*.10;
  half4 c=half4(tint*(ambient*.72+_LightColor0.rgb*(.18+diffuse*.85+back)*atten)+half3(.35,.41,.09)*rim+tint*FloorAdditionalDiffuse(i.world,n),1);UNITY_APPLY_FOG(i.fogCoord,c);return c;
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
