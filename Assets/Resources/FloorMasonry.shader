Shader "Floor/Masonry" {
 Properties {_Color("Stone tint",Color)=(.84,.79,.66,1) _MainTex("Original mineral grain",2D)="white"{} _Glossiness("Polish",Range(0,1))=.12 _Metallic("Metal",Range(0,1))=0 _CourseScale("Masonry joints per metre",Float)=1.7}
 SubShader {Tags {"RenderType"="Opaque"} LOD 150
 Pass {Tags {"LightMode"="ForwardBase"} CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 sampler2D _MainTex;fixed4 _Color;float _CourseScale;
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;UNITY_FOG_COORDS(2) SHADOW_COORDS(3)};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);
  half3 n=normalize(i.normal),a=abs(n);float horizontal=a.x>a.z?i.world.z:i.world.x;
  float row=floor(i.world.y*_CourseScale),x=horizontal*_CourseScale*.52+fmod(abs(row),2)*.5;
  float2 grid=frac(float2(x,i.world.y*_CourseScale));float joint=min(min(grid.x,1-grid.x),min(grid.y,1-grid.y));
  half mortar=1-smoothstep(.008,.035,joint);mortar*=saturate(1-a.y*2);
  half grain=tex2D(_MainTex,i.world.xz*.22+i.world.yy*.16).r;
  half course=.96+.055*sin(floor(x)*17.13+row*7.71);half3 stone=_Color.rgb*grain*course*(1-mortar*.19);
  half3 light=normalize(UnityWorldSpaceLightDir(i.world));half shade=saturate(dot(n,light));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.16,.16,.15));
  fixed4 col=fixed4(stone*(ambient*.64+_LightColor0.rgb*(.24+shade*.72)*atten),1);UNITY_APPLY_FOG(i.fogCoord,col);return col;
 } ENDCG } } Fallback "Mobile/Diffuse"
}
