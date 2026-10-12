Shader "Floor/Masonry" {
 Properties {_Color("Stone tint",Color)=(.84,.79,.66,1) _MainTex("Original mineral grain",2D)="white"{} _Glossiness("Polish",Range(0,1))=.12 _Metallic("Metal",Range(0,1))=0 _CourseScale("Masonry joints per metre",Float)=1.7 _WoodGrain("Carved entrance timber",Range(0,1))=0 _RoomGlow("Warm entrance fill",Range(0,1))=0 _RoomOrigin("Entrance origin",Vector)=(0,0,0,0) _RoomPeriod("Repeated floor height",Float)=0}
 SubShader {Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"} LOD 150
 Pass {Tags {"LightMode"="UniversalForward"} HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT
 #include "FloorUrpLighting.hlsl"
 sampler2D _MainTex;half4 _Color;float _CourseScale,_RoomGlow,_RoomPeriod,_WoodGrain,_Glossiness;float4 _RoomOrigin;
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) SHADOW_COORDS(3) half2 surface:TEXCOORD4;};
 v2f vert(appdata_full v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);o.surface=half2(step(.24,v.color.a)*(1-step(.26,v.color.a)),step(.49,v.color.a)*(1-step(.51,v.color.a)));UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 half4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);
  half3 n=normalize(i.normal),a=abs(n);float horizontal=a.x>a.z?i.world.z:i.world.x;
  float row=floor(i.world.y*_CourseScale),x=horizontal*_CourseScale*.52+fmod(abs(row),2)*.5;
  float2 grid=frac(float2(x,i.world.y*_CourseScale));float joint=min(min(grid.x,1-grid.x),min(grid.y,1-grid.y));
  half mortar=1-smoothstep(.008,.035,joint);mortar*=saturate(1-a.y*2);
  half grain=tex2D(_MainTex,float2(.005+frac(i.world.x*.22+i.world.y*.16)*.49,.005+frac(i.world.z*.22+i.world.y*.16)*.99)).r;
  float fibre=i.world.x*95+sin(i.world.y*2.1+i.world.x*5)*2.2+sin(i.world.y*.7)*1.2;half wood=.78+.10*sin(fibre)+.065*sin(fibre*2.13);
  grain=lerp(grain,wood,_WoodGrain);mortar*=1-_WoodGrain;
  half course=.99+.018*sin(floor(x)*17.13+row*7.71);half3 stone=_Color.rgb*grain*course*(1-mortar*.09);
  half3 light=normalize(UnityWorldSpaceLightDir(i.world));half shade=saturate(dot(n,light));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.16,.16,.15));
  // Bounded authored lantern fill shared by repeated tower floors. No additional
  // realtime lights, per-renderer blocks, texture samplers or reflection cameras.
  float3 room=i.world-_RoomOrigin.xyz;room.x=(room.x-.72)/1.65;if(_RoomPeriod>0)room.y-=floor(room.y/_RoomPeriod)*_RoomPeriod;
  half inside=saturate((1.2-abs(room.x))*4)*saturate((room.z-.2)*3)*saturate((3.9-room.y)*3)*saturate((room.y-.45)*3);
  float3 left=float3(-.90,2.02,.32)-room,right=float3(.90,2.02,.32)-room,ceiling=float3(0,3.38,1.95)-room;
  left.x*=1.65;right.x*=1.65;half warm=inside*_RoomGlow*((.25+saturate(dot(n,left*rsqrt(max(dot(left,left),.0001)))))/(1+dot(left,left)*.7)+(.25+saturate(dot(n,right*rsqrt(max(dot(right,right),.0001)))))/(1+dot(right,right)*.7));
  ceiling.x*=1.65;half ceilingDiffuse=saturate(dot(n,ceiling*rsqrt(max(dot(ceiling,ceiling),.001))));warm+=inside*_RoomGlow*(.20+ceilingDiffuse)/(1+dot(ceiling,ceiling)*.65);
  half3 view=normalize(_WorldSpaceCameraPos.xyz-i.world),ray=reflect(-view,n);
  half ribbon=pow(saturate(1-abs(ray.x*.72+ray.z*.34-.20)),48);
  half spec=pow(saturate(dot(n,normalize(light+view))),52)*atten;
  half vein=1-smoothstep(.02,.08,abs(sin(i.world.x*1.6+i.world.z*.73+sin(i.world.z*1.7)*.65)));
  half3 fill=FloorAdditionalDiffuse(i.world,n);half3 base=stone*(ambient*.60+_LightColor0.rgb*(.18+shade*.88)*atten+half3(1,.58,.22)*warm+fill);
  half3 marble=_Color.rgb*grain*(1-vein*.12)*(ambient*.58+_LightColor0.rgb*(.20+shade*.68)*atten+fill)+spec*.45+ribbon*half3(.22,.20,.16);
  // Opaque architectural glazing uses bounded authored reflected sky and warm
  // interior light; it is not a second camera or a claimed realtime reflection.
  half sky=saturate(ray.y*.6+.5);half3 glazing=lerp(half3(.09,.18,.19),half3(.50,.63,.64),sky)*.40+half3(.43,.23,.075)+ribbon*half3(.60,.56,.40)+spec*.65;
  half4 col=half4(lerp(lerp(base,marble,i.surface.x),glazing,i.surface.y),1);UNITY_APPLY_FOG(i.fogCoord,col);return col;
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
