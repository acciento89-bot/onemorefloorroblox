#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.OneMoreFloor.Visuals;
public static class FloorArchitectureValidation
{
 public static void Validate(){var host=new GameObject("ArchitectureVisibilityFixture");var owned=new HashSet<UnityEngine.Object>();try{
  LiftArchitecture.Bay(host.transform,0);LiftArchitecture.Cabin(host.transform);
  var bay=host.transform.Find("ArchitecturalFloorBay");var wall=bay.Find("MasonryFacadeSupport").GetComponent<Renderer>().bounds;var door=bay.Find("RecessedTealDoor").GetComponent<Renderer>().bounds;
  if(wall.min.z<=door.min.z)throw new InvalidOperationException("FLOOR_ARCHITECTURE: solid masonry occludes the recessed doorway from the fixed negative-Z camera");
  var cabin=host.transform.Find("CopperLiftCabin");if(cabin.Find("CabinBaseDrum")==null)throw new InvalidOperationException("FLOOR_ARCHITECTURE: lift needs a shaped metal base instead of four floating tubular rings");
  if(cabin.Find("CabinBaseDrum").GetComponent<Renderer>().bounds.size.y<.75f)throw new InvalidOperationException("FLOOR_ARCHITECTURE: lower lift body has no substantial shaped drum");
  int crowns=0;foreach(Transform part in cabin)if(part.name=="CabinCrown")crowns++;
  if(crowns!=1)throw new InvalidOperationException("FLOOR_ARCHITECTURE: disconnected repeated tubular crowns remain");
  if(host.GetComponentsInChildren<Collider>().Length!=0)throw new InvalidOperationException("FLOOR_ARCHITECTURE: decoration added collision");
  var masonry=bay.Find("MasonryFacadeSupport").GetComponent<Renderer>().sharedMaterial;
  if(masonry.mainTexture==null||masonry.mainTexture.width*masonry.mainTexture.height>65536)throw new InvalidOperationException("FLOOR_ARCHITECTURE: masonry needs a bounded mineral finish");
  var deck=new GameObject("ArchitectureDeckProbe");deck.transform.SetParent(host.transform,false);PlatformArt.Build(deck.transform,4,0);
  var collider=deck.GetComponentInChildren<BoxCollider>();
  if(collider.size!=new Vector3(4.6f,.70f,3.6f)||collider.center!=new Vector3(0,.25f,0)||host.GetComponentsInChildren<Collider>().Length!=1)throw new InvalidOperationException("FLOOR_ARCHITECTURE: original landing collision changed");
  int vertices=0;var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
  foreach(var f in host.GetComponentsInChildren<MeshFilter>()){vertices+=f.sharedMesh.vertexCount;owned.Add(f.sharedMesh);foreach(var normal in f.sharedMesh.normals)if(normal.sqrMagnitude<.5f)throw new InvalidOperationException("FLOOR_ARCHITECTURE: invalid lighting normal "+f.name);}
  foreach(var renderer in host.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial.name.StartsWith("FloorArchitecture",StringComparison.Ordinal)){
   materials.Add(renderer.sharedMaterial);owned.Add(renderer.sharedMaterial);if(renderer.sharedMaterial.mainTexture!=null){textures.Add(renderer.sharedMaterial.mainTexture);owned.Add(renderer.sharedMaterial.mainTexture);}
  }
  if(vertices>=30000||materials.Count>12||textures.Count>2)throw new InvalidOperationException("FLOOR_ARCHITECTURE: bounded mobile geometry/material kit exceeded");
  MeshArt.Batch(host);foreach(var f in host.GetComponentsInChildren<MeshFilter>())owned.Add(f.sharedMesh);
  Debug.Log("FLOOR_ARCHITECTURE_PASS visible doorway shaped cabin unchanged collider vertices="+vertices+" materials="+materials.Count+" textures="+textures.Count);
 }finally{UnityEngine.Object.DestroyImmediate(host);}
 foreach(var asset in owned)if(asset!=null)throw new InvalidOperationException("FLOOR_ARCHITECTURE: owned generated asset survives root destruction "+asset.name);
 }
}
#endif
