Shader "Floor/CabinGlass" {
 Properties {[HideInInspector] _MainTex("Compatibility white texture",2D)="white"{} _Color("Clear teal glass",Color)=(.35,.64,.64,.09) _Glossiness("Polish",Range(0,1))=.8 _Metallic("Metal",Range(0,1))=0}
 SubShader {Tags {"Queue"="Transparent+10" "RenderType"="Transparent"} LOD 100 Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
 Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fog
 #include "UnityCG.cginc"
 fixed4 _Color;struct v2f{float4 pos:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;UNITY_FOG_COORDS(2)};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;o.normal=UnityObjectToWorldNormal(v.normal);UNITY_TRANSFER_FOG(o,o.pos);return o;}
 fixed4 frag(v2f i):SV_Target{half3 n=normalize(i.normal),view=normalize(_WorldSpaceCameraPos.xyz-i.world);half fresnel=pow(1-abs(dot(n,view)),4);half3 reflected=reflect(-view,n);half ribbon=pow(saturate(1-abs(reflected.x*.7+reflected.z*.3-.23)),34);fixed4 col=fixed4(_Color.rgb+half3(.23,.22,.15)*ribbon,_Color.a+fresnel*.12+ribbon*.055);UNITY_APPLY_FOG(i.fogCoord,col);return col;}
 ENDCG } }
}
