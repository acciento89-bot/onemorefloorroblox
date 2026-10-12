Shader "Floor/LightHalo" {
 Properties { [HideInInspector] _MainTex("Compatibility",2D)="white"{} _Color("Warm glow",Color)=(1,.6,.16,.32) _Glossiness("Compatibility",Range(0,1))=0 _EmissionColor("Compatibility",Color)=(0,0,0,1) }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent"} Blend SrcAlpha One ZWrite Off Cull Off
 Pass {CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 fixed4 _Color;struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.texcoord.xy;return o;}
 fixed4 frag(v2f i):SV_Target {half r=length((i.uv-.5)*2);half falloff=pow(saturate(1-r),2);return fixed4(_Color.rgb,_Color.a*falloff);}
 ENDCG }
 }
}
