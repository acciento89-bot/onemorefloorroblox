Shader "Floor/ArchitecturalMetal" {
 Properties { _Color("Metal tint",Color)=(.74,.51,.23,1) _MainTex("Original brushed finish",2D)="white"{} _Glossiness("Polish",Range(0,1))=.68 _Metallic("Metal",Range(0,1))=.72 _EmissionColor("Emission",Color)=(0,0,0,1) }
 SubShader { Tags {"RenderType"="Opaque"} LOD 150
 Pass { Tags {"LightMode"="ForwardBase"} CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #pragma multi_compile_fwdbase
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 sampler2D _MainTex;float4 _MainTex_ST;fixed4 _Color;half _Glossiness,_Metallic;
 struct metalInput {float4 vertex:POSITION;float3 normal:NORMAL;float2 texcoord:TEXCOORD0;fixed4 color:COLOR;};
 struct v2f {float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;UNITY_FOG_COORDS(3) SHADOW_COORDS(4) fixed4 tint:TEXCOORD5;};
 v2f vert(metalInput v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);o.tint=v.color;UNITY_TRANSFER_FOG(o,o.pos);TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target{
  UNITY_LIGHT_ATTENUATION(atten,i,i.world);
  half3 n=normalize(i.normal),view=normalize(_WorldSpaceCameraPos.xyz-i.world),light=normalize(UnityWorldSpaceLightDir(i.world));half3 ray=reflect(-view,n);
  // Authored sunset/softbox reflection gives polished brass readable bands on mobile,
  // without a reflection camera, per-frame cubemap or a scene-global probe.
  half horizon=pow(saturate(1-abs(ray.y+.10)),9);half ceiling=pow(saturate(ray.y*.5+.5),2);
  half ribbon=pow(saturate(1-abs(ray.x*.82+ray.z*.36-.27)),28);
  half3 reflected=lerp(half3(.08,.17,.19),half3(1.0,.84,.53),ceiling)+horizon*half3(.70,.49,.21)+ribbon*half3(.98,1.02,.86);
  half3 paint=lerp(half3(1,1,1),i.tint.rgb,step(.01,i.tint.a));half3 albedo=tex2D(_MainTex,i.uv).rgb*_Color.rgb*paint;
  half diffuse=saturate(dot(n,light));half spec=pow(saturate(dot(n,normalize(light+view))),lerp(20,96,_Glossiness));
  half3 ambient=max(ShadeSH9(half4(n,1)),half3(.13,.13,.12));
  half fresnel=pow(1-saturate(dot(n,view)),4);
  half3 result=albedo*(ambient*.48+_LightColor0.rgb*diffuse*atten*.66)+reflected*lerp(albedo,half3(.91,.79,.50),.28)*(.20+_Metallic*.48)+_LightColor0.rgb*spec*atten*2.1+fresnel*half3(.12,.11,.08);
  result=result/(1+result*.38);fixed4 col=fixed4(result,1);UNITY_APPLY_FOG(i.fogCoord,col);return col;
 }
 ENDCG }
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
