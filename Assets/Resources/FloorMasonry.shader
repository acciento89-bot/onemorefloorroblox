Shader "Floor/Masonry" {
 Properties {_Color("Stone tint",Color)=(.84,.79,.66,1) _MainTex("Original mineral grain",2D)="white"{} _Glossiness("Polish",Range(0,1))=.12 _Metallic("Metal",Range(0,1))=0 _CourseScale("Masonry joints per metre",Float)=1.7 _WoodGrain("Carved entrance timber",Range(0,1))=0 _RoomGlow("Warm entrance fill",Range(0,1))=0 _RoomOrigin("Entrance origin",Vector)=(0,0,0,0) _RoomPeriod("Repeated floor height",Float)=0}
 SubShader {Tags {"RenderType"="Opaque"} LOD 150
 Pass {Tags {"LightMode"="ForwardBase"} CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 sampler2D _MainTex;fixed4 _Color;float _CourseScale,_RoomGlow,_RoomPeriod,_WoodGrain;float4 _RoomOrigin;
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) SHADOW_COORDS(3)};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);
  half3 n=normalize(i.normal),a=abs(n);float horizontal=a.x>a.z?i.world.z:i.world.x;
  float row=floor(i.world.y*_CourseScale),x=horizontal*_CourseScale*.52+fmod(abs(row),2)*.5;
  float2 grid=frac(float2(x,i.world.y*_CourseScale));float joint=min(min(grid.x,1-grid.x),min(grid.y,1-grid.y));
  half mortar=1-smoothstep(.008,.035,joint);mortar*=saturate(1-a.y*2);
  half grain=tex2D(_MainTex,i.world.xz*.22+i.world.yy*.16).r;
  float fibre=i.world.x*95+sin(i.world.y*2.1+i.world.x*5)*2.2+sin(i.world.y*.7)*1.2;half wood=.78+.10*sin(fibre)+.065*sin(fibre*2.13);
  grain=lerp(grain,wood,_WoodGrain);mortar*=1-_WoodGrain;
  half course=.99+.018*sin(floor(x)*17.13+row*7.71);half3 stone=_Color.rgb*grain*course*(1-mortar*.09);
  half3 light=normalize(UnityWorldSpaceLightDir(i.world));half shade=saturate(dot(n,light));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.16,.16,.15));
  // Bounded authored lantern fill shared by repeated tower floors. No additional
  // realtime lights, per-renderer blocks, texture samplers or reflection cameras.
  float3 room=i.world-_RoomOrigin.xyz;room.x=(room.x-.72)/1.65;if(_RoomPeriod>0)room.y-=floor(room.y/_RoomPeriod)*_RoomPeriod;
  half inside=saturate((1.2-abs(room.x))*4)*saturate((room.z-.2)*3)*saturate((3.9-room.y)*3)*saturate((room.y-.45)*3);
  float3 left=float3(-.90,2.02,.32)-room,right=float3(.90,2.02,.32)-room;
  left.x*=1.65;right.x*=1.65;half warm=inside*_RoomGlow*((.25+saturate(dot(n,left*rsqrt(max(dot(left,left),.0001)))))/(1+dot(left,left)*.7)+(.25+saturate(dot(n,right*rsqrt(max(dot(right,right),.0001)))))/(1+dot(right,right)*.7));
  fixed4 col=fixed4(stone*(ambient*.60+_LightColor0.rgb*(.18+shade*.88)*atten+half3(1,.58,.22)*warm),1);UNITY_APPLY_FOG(i.fogCoord,col);return col;
 } ENDCG }
 Pass {Name "SHADOWCASTER" Tags {"LightMode"="ShadowCaster"} ZWrite On ZTest LEqual
 CGPROGRAM
 #pragma vertex vertShadow
 #pragma fragment fragShadow
 #pragma multi_compile_shadowcaster
 #include "UnityCG.cginc"
 struct shadowOutput {V2F_SHADOW_CASTER;};
 shadowOutput vertShadow(appdata_base v){shadowOutput o;TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);return o;}
 fixed4 fragShadow(shadowOutput i):SV_Target {SHADOW_CASTER_FRAGMENT(i)}
 ENDCG }
 } Fallback "Mobile/Diffuse"
}
