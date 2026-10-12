Shader "Floor/Sky" { Properties { _MainTex("Original distant canal panorama",2D)="white"{} } SubShader { Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"} Cull Off ZWrite Off Pass { CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;float4 _MainTex_TexelSize;struct v2f{float4 pos:SV_POSITION;};v2f vert(float4 vertex:POSITION){v2f o;o.pos=UnityObjectToClipPos(vertex);return o;}
fixed4 frag(v2f i):SV_Target{
 // The approved camera is fixed. Fit this distant painted environment to the
 // actual framebuffer, preserving circles and architecture in both orientations.
 float viewAspect=_ScreenParams.x/_ScreenParams.y;float imageAspect=_MainTex_TexelSize.z/_MainTex_TexelSize.w;
 float2 scale=viewAspect>imageAspect?float2(1,imageAspect/viewAspect):float2(viewAspect/imageAspect,1);
 float2 uv=(i.pos.xy/_ScreenParams.xy-.5)*scale+.5;
 return tex2D(_MainTex,uv);
}
ENDCG } } }
