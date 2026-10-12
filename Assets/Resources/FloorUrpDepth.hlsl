#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct FloorDepthInput {float4 vertex:POSITION;};
struct FloorDepthOutput {float4 pos:SV_POSITION;};
FloorDepthOutput vertDepth(FloorDepthInput i){FloorDepthOutput o;o.pos=TransformObjectToHClip(i.vertex.xyz);return o;}
half4 fragDepth(FloorDepthOutput i):SV_Target{return 0;}
