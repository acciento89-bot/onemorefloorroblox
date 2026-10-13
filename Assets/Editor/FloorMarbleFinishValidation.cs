#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorMarbleFinishValidation
{
 public static void Validate()
 {
  var root=new GameObject("ActualPolishedMarbleFixture");
  try{
   var cube=Resources.Load<Cubemap>("Rendering/CapturedAtriumReflection");
   foreach(var kind in new[]{FloorArchitectureSurface.Stone,FloorArchitectureSurface.Trim}){
    var finish=FloorArchitecturePalette.Get(root.transform,kind);
    if(!finish.HasProperty("_ReflectionCube")||finish.GetTexture("_ReflectionCube")!=cube||finish.GetFloat("_ReflectionStrength")<.15f)throw new InvalidOperationException("FLOOR_MARBLE: shipping stone/trim must bind the shared original cubemap for the marked polished floor only");
   }
   Debug.Log("FLOOR_MARBLE_FINISH_PASS actual palette / shared original cube / runtime pixels validated separately");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
 }
}
#endif
