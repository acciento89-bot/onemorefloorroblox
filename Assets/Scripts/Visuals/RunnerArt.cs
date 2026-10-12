using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Visuals
{
 public static class RunnerArt
 {
  public static RunnerAnimator Build(Transform parent)
  {
   var root=new GameObject("HoodieRunner",typeof(RunnerAnimator));root.transform.SetParent(parent,false);
   var animator=root.GetComponent<RunnerAnimator>();
   // All mutable materials and the detail texture belong to this avatar.
   var skin=Material(root,"LiftRunnerWarmSkin",new Color(.97f,.72f,.50f),.22f);
   var pants=Material(root,"RunnerBrownTravelCanvas",new Color(.19f,.20f,.16f),.08f);
   var ivory=Material(root,"LiftRunnerIvoryCanvas",new Color(.87f,.83f,.72f),.14f);
   var dark=Material(root,"LiftRunnerSeamsAndEyes",new Color(.045f,.055f,.067f),.12f);
   var teal=Material(root,"RunnerTealAdventureHoodie",new Color(.035f,.29f,.30f),.10f);
   var amber=Material(root,"RunnerAmberTrim",new Color(.91f,.58f,.14f),.20f);
   var hair=Material(root,"LiftRunnerChestnutHair",new Color(.34f,.17f,.07f),.20f);
   var gold=Material(root,"LiftRunnerPremiumThread",new Color(1,.88f,.39f),.22f);
   hair.mainTexture=HairDetail(root);var weave=ClothDetail(root);teal.mainTexture=weave;pants.mainTexture=weave;ivory.mainTexture=weave;animator.Hoodie=teal;
   animator.Hips=Pivot(root.transform,"Hips",new Vector3(0,.711f,0));animator.Hips.localScale=new Vector3(1,.84f,1);

   RunnerTailorGeometry.Tunic(animator.Hips,teal);

   var hood=new RunnerMeshBuilder(animator.Hips,"WhiteDrapedHood",ivory);
   hood.Sweep(new[]{new Vector3(-.18f,.525f,.015f),new Vector3(-.20f,.56f,-.08f),new Vector3(-.15f,.615f,-.155f),new Vector3(0,.64f,-.17f),new Vector3(.15f,.615f,-.155f),new Vector3(.20f,.56f,-.08f),new Vector3(.18f,.525f,.015f)},new[]{.043f,.063f,.076f,.082f,.076f,.063f,.043f},new[]{.025f,.038f,.043f,.047f,.043f,.038f,.025f},10);
   for(int side=-1;side<=1;side+=2){hood.Sweep(new[]{new Vector3(side*.086f,.548f,.095f),new Vector3(side*.075f,.48f,.159f),new Vector3(side*.078f,.392f,.167f)},new[]{.006f,.005f,.005f},new[]{.006f,.005f,.005f},6);hood.Pebble(new Vector3(side*.078f,.384f,.168f),new Vector3(.014f,.023f,.014f),6);}
   hood.Finish();

   RunnerTailorGeometry.TravelPack(animator.Hips,pants);
   var seams=new RunnerMeshBuilder(animator.Hips,"HoodieAndWaistSeams",dark);
   seams.Stitch(new Vector3(-.10f,.29f,.181f),new Vector3(-.15f,.22f,.18f),.003f);
   seams.Stitch(new Vector3(.10f,.29f,.181f),new Vector3(.15f,.22f,.18f),.003f);
   seams.Stitch(new Vector3(0,.10f,.129f),new Vector3(0,-.025f,.133f),.003f);
   seams.Sweep(new[]{new Vector3(0,.115f,.164f),new Vector3(0,.305f,.182f),new Vector3(0,.43f,.158f),new Vector3(0,.54f,.108f)},new[]{.004f,.004f,.004f,.004f},new[]{.003f,.003f,.003f,.003f},6);seams.Finish();
   var trim=new RunnerMeshBuilder(animator.Hips,"AmberLiftTrim",amber);
   trim.Stitch(new Vector3(0,.44f,.166f),new Vector3(0,.466f,.165f),.007f);
   for(int side=-1;side<=1;side+=2)trim.Sweep(new[]{new Vector3(side*.105f,.53f,.088f),new Vector3(side*.17f,.523f,.090f),new Vector3(side*.216f,.488f,.080f)},new[]{.004f,.004f,.003f},new[]{.003f,.003f,.003f},6);
   // Three ascending lift bars give this figure its own city-route identity.
   for(int bar=0;bar<3;bar++){float x=-.035f+bar*.035f;trim.Stitch(new Vector3(x,.275f,-.339f),new Vector3(x,.32f+bar*.025f,-.339f),.007f);}
   // Tiny buckle and flap stitching remain in the existing amber detail batch.
   for(int side=-1;side<=1;side+=2){trim.Stitch(new Vector3(side*.123f,.377f,.192f),new Vector3(side*.123f,.414f,.193f),.006f);trim.Stitch(new Vector3(side*.10f,.438f,-.355f),new Vector3(side*.10f,.477f,-.363f),.005f);}
   trim.Finish();

   for(int side=-1;side<=1;side+=2){BuildLeg(animator,side,pants,ivory,amber,dark);BuildArm(animator,side,teal,skin,dark,amber);}
   BuildHead(animator,skin,ivory,dark,hair);
   for(int style=4;style<8;style++){
    var crest=Pivot(animator.Hips,"PremiumCrest"+style,new Vector3(0,.49f,-.376f));
    var thread=new RunnerMeshBuilder(crest,"EmbroideredThread",gold);int rays=style==5?4:style==7?5:3;
    for(int k=0;k<rays;k++){float angle=(k*360f/rays+style*9)*Mathf.Deg2Rad;thread.Stitch(new Vector3(Mathf.Sin(angle)*.022f,Mathf.Cos(angle)*.022f,-.006f),new Vector3(Mathf.Sin(angle)*.083f,Mathf.Cos(angle)*.083f,-.009f),.005f);}
    thread.Finish();crest.gameObject.SetActive(false);
   }
   animator.CaptureRestPose();return animator;
  }

  static void BuildLeg(RunnerAnimator animator,int side,Material pants,Material ivory,Material amber,Material dark)
  {
   var leg=Pivot(animator.Hips,side<0?"LeftLeg":"RightLeg",new Vector3(side*.115f,.005f,0));
   var knee=Pivot(leg,side<0?"LeftKnee":"RightKnee",new Vector3(0,-.35f,.012f));
   if(side<0){animator.LeftLeg=leg;animator.LeftKnee=knee;}else{animator.RightLeg=leg;animator.RightKnee=knee;}
   var thigh=new RunnerMeshBuilder(leg,"CargoThigh",pants);
   thigh.Profile(Vector3.zero,new[]{-.37f,-.30f,-.16f,-.025f,.03f},new[]{.092f,.105f,.12f,.124f,.10f},new[]{.090f,.101f,.110f,.109f,.083f},12,.72f);
   thigh.Loft(new[]{new Vector3(side*.109f,-.265f,.006f),new Vector3(side*.125f,-.24f,.01f),new Vector3(side*.128f,-.13f,.01f),new Vector3(side*.112f,-.105f,.007f)},new[]{new Vector2(.013f,.05f),new Vector2(.022f,.071f),new Vector2(.023f,.072f),new Vector2(.009f,.055f)},8,.6f);thigh.Finish();
   var pocket=new RunnerMeshBuilder(leg,"CargoPocketFlap",dark);
   pocket.Stitch(new Vector3(side*.151f,-.137f,-.05f),new Vector3(side*.151f,-.137f,.067f),.004f);pocket.Stitch(new Vector3(side*.114f,-.30f,.067f),new Vector3(side*.112f,-.21f,.10f),.003f);pocket.Finish();
   var shin=new RunnerMeshBuilder(knee,"CargoShinAndCuff",pants);
   shin.Profile(new Vector3(0,0,-.012f),new[]{-.325f,-.29f,-.17f,-.04f,.015f},new[]{.068f,.072f,.080f,.092f,.083f},new[]{.071f,.075f,.087f,.096f,.077f},12,.76f);
   shin.Profile(new Vector3(0,0,-.012f),new[]{-.286f,-.266f,-.242f},new[]{.079f,.086f,.083f},new[]{.083f,.089f,.087f},12,.68f);shin.Finish();
   var footOrigin=new Vector3(0,-.36f,-.007f);
   var white=new RunnerMeshBuilder(knee,"WhiteSneakerLast",ivory);
   white.Shoe(footOrigin,-.13f,-.081f,0);white.Shoe(footOrigin,-.081f,.025f,.035f);
   // Structured ankle collar and raised tongue turn the existing footwear last
   // into travel boots without moving the authoritative feet or adding batches.
   white.Profile(footOrigin+new Vector3(0,0,-.035f),new[]{-.021f,.026f,.073f,.092f},new[]{.077f,.079f,.076f,.068f},new[]{.064f,.070f,.061f,.051f},12,.69f);
   white.Loft(new[]{footOrigin+new Vector3(0,.007f,.073f),footOrigin+new Vector3(0,.037f,.053f),footOrigin+new Vector3(0,.082f,.024f)},new[]{new Vector2(.042f,.010f),new Vector2(.039f,.012f),new Vector2(.030f,.009f)},10,.62f);
   for(int lace=0;lace<3;lace++){float z=.025f+lace*.040f;white.Stitch(footOrigin+new Vector3(-.064f,.026f-lace*.006f,z),footOrigin+new Vector3(.064f,.026f-lace*.006f,z),.006f);}var bootObject=white.Finish();var bootMesh=bootObject.GetComponent<MeshFilter>().sharedMesh;var bootPoints=bootMesh.vertices;var bootTint=new Color[bootPoints.Length];for(int i=0;i<bootPoints.Length;i++)bootTint[i]=bootPoints[i].y>footOrigin.y-.074f?new Color(.38f,.25f,.14f,.5f):Color.white;bootMesh.colors=bootTint;
   var accent=new RunnerMeshBuilder(knee,"AmberSneakerPanels",amber);
   accent.Sweep(new[]{footOrigin+new Vector3(side*.082f,-.040f,-.09f),footOrigin+new Vector3(side*.106f,-.048f,.015f),footOrigin+new Vector3(side*.106f,-.046f,.105f)},new[]{.027f,.029f,.017f},new[]{.006f,.006f,.005f},6);
   accent.Profile(footOrigin+new Vector3(0,0,-.079f),new[]{-.075f,-.027f,.030f},new[]{.075f,.079f,.070f},new[]{.027f,.030f,.023f},10,.7f);
   accent.Sweep(new[]{footOrigin+new Vector3(-.067f,.088f,-.049f),footOrigin+new Vector3(0,.095f,-.089f),footOrigin+new Vector3(.067f,.088f,-.049f)},new[]{.008f,.009f,.008f},new[]{.006f,.006f,.006f},8);accent.Finish();
   var tread=new RunnerMeshBuilder(knee,"SneakerSoleEdge",dark);
   for(int i=0;i<3;i++)tread.Stitch(footOrigin+new Vector3(-.084f,-.132f,-.065f+i*.083f),footOrigin+new Vector3(.084f,-.132f,-.065f+i*.083f),.003f);tread.Finish();
  }

  static void BuildArm(RunnerAnimator animator,int side,Material teal,Material skin,Material dark,Material amber)
  {
   var arm=Pivot(animator.Hips,side<0?"LeftArm":"RightArm",new Vector3(side*.236f,.505f,0));arm.localRotation=Quaternion.Euler(0,0,side*11);
   var elbow=Pivot(arm,side<0?"LeftElbow":"RightElbow",new Vector3(0,-.265f,.008f));elbow.localRotation=Quaternion.Euler(-12,0,0);
   if(side<0){animator.LeftArm=arm;animator.LeftElbow=elbow;}else{animator.RightArm=arm;animator.RightElbow=elbow;}
   var sleeve=new RunnerMeshBuilder(arm,"TailoredRolledSleeve",teal);
   sleeve.Profile(Vector3.zero,new[]{-.28f,-.245f,-.20f,-.07f,.025f,.07f},new[]{.080f,.086f,.089f,.11f,.095f,.045f},new[]{.080f,.088f,.091f,.106f,.091f,.045f},12,.83f);
   sleeve.Profile(Vector3.zero,new[]{-.285f,-.255f,-.233f},new[]{.085f,.096f,.092f},new[]{.085f,.096f,.092f},12,.72f);sleeve.Finish();
   var limb=new RunnerMeshBuilder(elbow,"ForearmAndHand",skin);
   limb.Profile(Vector3.zero,new[]{-.245f,-.20f,-.10f,.01f},new[]{.048f,.055f,.068f,.072f},new[]{.047f,.052f,.067f,.072f},12);
   limb.Loft(new[]{new Vector3(0,-.34f,.036f),new Vector3(0,-.322f,.021f),new Vector3(0,-.28f,.015f),new Vector3(0,-.245f,.003f)},new[]{new Vector2(.039f,.022f),new Vector2(.053f,.030f),new Vector2(.058f,.039f),new Vector2(.046f,.035f)},10,.72f);
   // Thumb and curled finger ridges share the hand mesh and no collider.
   limb.Sweep(new[]{new Vector3(-side*.046f,-.268f,.024f),new Vector3(-side*.070f,-.289f,.05f),new Vector3(-side*.064f,-.319f,.058f)},new[]{.021f,.023f,.013f},new[]{.020f,.018f,.011f},7);
   for(int finger=0;finger<3;finger++){float x=-.028f+finger*.027f;limb.Sweep(new[]{new Vector3(x,-.304f,.044f),new Vector3(x,-.329f,.056f),new Vector3(x,-.344f,.035f)},new[]{.013f,.014f,.010f},new[]{.011f,.012f,.009f},6);}var handObject=limb.Finish();var handMesh=handObject.GetComponent<MeshFilter>().sharedMesh;var handPoints=handMesh.vertices;var handTint=new Color[handPoints.Length];for(int i=0;i<handPoints.Length;i++){var p=handPoints[i];handTint[i]=p.y<-.244f&&p.y>-.331f&&p.z<.055f?new Color(.045f,.055f,.052f,.5f):Color.white;}handMesh.colors=handTint;
   var cuffTrim=new RunnerMeshBuilder(arm,"AmberSleevePiping",amber);
   cuffTrim.Profile(Vector3.zero,new[]{-.263f,-.252f},new[]{.096f,.096f},new[]{.096f,.096f},12,.72f);cuffTrim.Finish();
   var band=new RunnerMeshBuilder(elbow,"WristBand",teal);band.Profile(Vector3.zero,new[]{-.245f,-.221f,-.11f,.025f},new[]{.062f,.067f,.082f,.080f},new[]{.059f,.066f,.079f,.079f},12,.86f);band.Finish();
  }

  static void BuildHead(RunnerAnimator animator,Material skin,Material ivory,Material dark,Material hair)
  {
   animator.Head=Pivot(animator.Hips,"Head",new Vector3(0,.815f,.003f));animator.Head.localScale=new Vector3(1.26f,1.32f,1.26f);
   var neck=new RunnerMeshBuilder(animator.Hips,"Neck",skin);neck.Profile(new Vector3(0,0,.008f),new[]{.56f,.65f,.68f},new[]{.071f,.068f,.073f},new[]{.071f,.068f,.073f},12);neck.Finish();
   var face=new RunnerMeshBuilder(animator.Head,"FaceSkin",skin);
   face.Profile(new Vector3(0,0,.015f),new[]{-.215f,-.175f,-.085f,.03f,.12f,.195f,.225f},new[]{.085f,.138f,.181f,.190f,.187f,.151f,.085f},new[]{.069f,.109f,.142f,.154f,.145f,.115f,.06f},24,1f);var faceObject=face.Finish();var faceMesh=faceObject.GetComponent<MeshFilter>().sharedMesh;var facePoints=faceMesh.vertices;var faceTint=new Color[facePoints.Length];for(int i=0;i<facePoints.Length;i++){var p=facePoints[i];float blush=Mathf.Exp(-((Mathf.Abs(p.x)-.14f)*(Mathf.Abs(p.x)-.14f)/.004f+(p.y+.065f)*(p.y+.065f)/.004f))*Mathf.Clamp01(p.z*8);faceTint[i]=Color.Lerp(Color.white,new Color(1,.80f,.79f),blush*.65f);}faceMesh.colors=faceTint;
   var features=new RunnerMeshBuilder(animator.Head,"FaceFeatures",skin);
   features.Loft(new[]{new Vector3(0,-.082f,.170f),new Vector3(0,-.055f,.178f),new Vector3(0,-.01f,.174f),new Vector3(0,.014f,.165f)},new[]{new Vector2(.025f,.012f),new Vector2(.027f,.018f),new Vector2(.021f,.012f),new Vector2(.013f,.010f)},8);
   for(int side=-1;side<=1;side+=2)features.Pebble(new Vector3(side*.196f,-.028f,-.005f),new Vector3(.054f,.096f,.059f),10);features.Finish();
   var eyes=new RunnerMeshBuilder(animator.Head,"EyeWhites",ivory);var expression=new RunnerMeshBuilder(animator.Head,"EyesAndExpression",dark);
   // Thin lenses follow the cheek curvature. The far eye sits behind the nose
   // in the fixed side view instead of appearing as a second floating eyeball.
   for(int side=-1;side<=1;side+=2){var orientation=Quaternion.Euler(0,side*23f,0);var normal=orientation*Vector3.forward;var center=new Vector3(side*.079f,.021f,.154f);eyes.Pebble(center,new Vector3(.077f,.091f,.014f),10,orientation);eyes.Pebble(center+normal*.014f+new Vector3(-side*.003f,.012f,0),new Vector3(.011f,.015f,.005f),8,orientation);expression.Pebble(center+normal*.010f+Vector3.down*.003f,new Vector3(.039f,.058f,.006f),8,orientation);expression.Sweep(new[]{new Vector3(side*.043f,.081f,.164f),new Vector3(side*.074f,.089f,.163f),new Vector3(side*.109f,.080f,.153f)},new[]{.008f,.011f,.006f},new[]{.004f,.005f,.003f},6);}
   expression.Sweep(new[]{new Vector3(-.042f,-.128f,.145f),new Vector3(0,-.136f,.153f),new Vector3(.043f,-.120f,.145f)},new[]{.003f,.004f,.003f},new[]{.002f,.002f,.002f},5);var eyeObject=eyes.Finish();var eyeMesh=eyeObject.GetComponent<MeshFilter>().sharedMesh;var eyeTint=new Color[eyeMesh.vertexCount];for(int i=0;i<eyeTint.Length;i++)eyeTint[i]=new Color(.96f,.97f,.95f,.5f);eyeMesh.colors=eyeTint;expression.Finish();
   RunnerTailorGeometry.Hair(animator.Head,hair);
  }

  static Material Material(GameObject owner,string name,Color color,float gloss)
  {var material=new Material(Resources.Load<Material>("Materials/Surface")){name=name,color=color};material.shader=Resources.Load<Shader>("FloorRunnerSurface");material.SetFloat("_SurfaceKind",name.Contains("Skin")?1:name.Contains("Hair")?2:0);material.SetFloat("_Glossiness",gloss);ArtLifetime.Own(owner,material);return material;}
  static Texture2D ClothDetail(GameObject owner)
  {
   const int size=64;var pixels=new Color[size*size];for(int y=0;y<size;y++)for(int x=0;x<size;x++){float threads=((x+y)%2==0?.025f:-.025f)+Mathf.Sin(x*Mathf.PI*.5f)*.013f+Mathf.Cos(y*Mathf.PI*.5f)*.013f;float value=.96f+threads;pixels[y*size+x]=new Color(value,value,value);}
   var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="RunnerWovenCanvasDetail",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear};texture.SetPixels(pixels);texture.Apply(true,true);ArtLifetime.Own(owner,texture);return texture;
  }
  static Texture2D HairDetail(GameObject owner)
  {
   const int size=64;var pixels=new Color[size*size];
   for(int y=0;y<size;y++)for(int x=0;x<size;x++){float u=(float)x/size,v=(float)y/size;float strand=Mathf.Sin(u*Mathf.PI*24+Mathf.Sin(v*Mathf.PI*2)*.8f),fine=Mathf.Sin(u*Mathf.PI*54+v*Mathf.PI*4);float value=.84f+strand*.09f+fine*.028f;pixels[y*size+x]=new Color(value,value,value,1);}
   var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="RunnerHairStrandDetail",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear};texture.SetPixels(pixels);texture.Apply(true,true);ArtLifetime.Own(owner,texture);return texture;
  }
  static Transform Pivot(Transform parent,string name,Vector3 at)
  {var transform=new GameObject(name).transform;transform.SetParent(parent,false);transform.localPosition=at;return transform;}
 }
}
