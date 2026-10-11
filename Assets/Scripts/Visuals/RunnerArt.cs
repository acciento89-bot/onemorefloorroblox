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
   var skin=Material(root,"LiftRunnerWarmSkin",new Color(.86f,.58f,.36f),.22f);
   var pants=Material(root,"RunnerBrownTravelCanvas",new Color(.29f,.20f,.115f),.08f);
   var ivory=Material(root,"LiftRunnerIvoryCanvas",new Color(.94f,.955f,.90f),.14f);
   var dark=Material(root,"LiftRunnerSeamsAndEyes",new Color(.045f,.055f,.067f),.12f);
   var teal=Material(root,"RunnerTealAdventureHoodie",new Color(.035f,.29f,.30f),.10f);
   var amber=Material(root,"RunnerAmberTrim",new Color(.91f,.58f,.14f),.20f);
   var hair=Material(root,"LiftRunnerChestnutHair",new Color(.30f,.145f,.070f),.20f);
   var gold=Material(root,"LiftRunnerPremiumThread",new Color(1,.88f,.39f),.22f);
   hair.mainTexture=HairDetail(root);animator.Hoodie=teal;
   animator.Hips=Pivot(root.transform,"Hips",new Vector3(0,.85f,0));

   var hoodie=new RunnerMeshBuilder(animator.Hips,"TailoredLiftHoodie",teal);
   hoodie.Profile(Vector3.zero,new[]{.045f,.10f,.22f,.40f,.51f,.57f},new[]{.205f,.215f,.212f,.233f,.245f,.155f},new[]{.133f,.142f,.147f,.150f,.135f,.095f},16,.72f);
   // The front pocket has a flat opening and tapered curved lower corners.
   hoodie.Loft(new[]{new Vector3(0,.15f,.145f),new Vector3(0,.19f,.163f),new Vector3(0,.28f,.168f),new Vector3(0,.305f,.156f)},new[]{new Vector2(.11f,.008f),new Vector2(.153f,.018f),new Vector2(.143f,.018f),new Vector2(.10f,.008f)},12,.62f);
   hoodie.Profile(Vector3.zero,new[]{.04f,.075f,.10f},new[]{.207f,.218f,.211f},new[]{.136f,.146f,.141f},16,.72f);hoodie.Finish();

   var hood=new RunnerMeshBuilder(animator.Hips,"WhiteDrapedHood",ivory);
   hood.Sweep(new[]{new Vector3(-.18f,.525f,.015f),new Vector3(-.20f,.56f,-.08f),new Vector3(-.15f,.615f,-.155f),new Vector3(0,.64f,-.17f),new Vector3(.15f,.615f,-.155f),new Vector3(.20f,.56f,-.08f),new Vector3(.18f,.525f,.015f)},new[]{.043f,.063f,.076f,.082f,.076f,.063f,.043f},new[]{.025f,.038f,.043f,.047f,.043f,.038f,.025f},10);
   for(int side=-1;side<=1;side+=2){hood.Sweep(new[]{new Vector3(side*.086f,.548f,.095f),new Vector3(side*.075f,.48f,.159f),new Vector3(side*.078f,.392f,.167f)},new[]{.006f,.005f,.005f},new[]{.006f,.005f,.005f},6);hood.Pebble(new Vector3(side*.078f,.384f,.168f),new Vector3(.014f,.023f,.014f),6);}
   hood.Finish();

   var hipFabric=new RunnerMeshBuilder(animator.Hips,"CargoWaist",pants);
   hipFabric.Profile(Vector3.zero,new[]{-.055f,.015f,.10f},new[]{.19f,.218f,.205f},new[]{.113f,.133f,.126f},14,.72f);
   // A tailored travel pack shares the existing brown canvas batch: shoulder straps,
   // broad flap and side pockets are real depth, rather than another flat crest.
   hipFabric.Profile(new Vector3(0,0,-.235f),new[]{.145f,.185f,.31f,.465f,.52f},new[]{.115f,.155f,.165f,.161f,.127f},new[]{.058f,.086f,.094f,.093f,.072f},14,.62f);
   hipFabric.Profile(new Vector3(0,0,-.252f),new[]{.44f,.485f,.54f,.555f},new[]{.158f,.176f,.170f,.125f},new[]{.093f,.115f,.105f,.078f},14,.64f);
   for(int side=-1;side<=1;side+=2){
    hipFabric.Profile(new Vector3(side*.158f,0,-.224f),new[]{.17f,.205f,.31f,.355f},new[]{.028f,.045f,.048f,.029f},new[]{.045f,.073f,.070f,.041f},10,.60f);
    hipFabric.Sweep(new[]{new Vector3(side*.112f,.27f,.157f),new Vector3(side*.121f,.405f,.174f),new Vector3(side*.137f,.52f,.174f),new Vector3(side*.158f,.575f,-.065f),new Vector3(side*.127f,.486f,-.17f)},new[]{.018f,.020f,.022f,.022f,.019f},new[]{.009f,.009f,.010f,.011f,.010f},8);
   }
   hipFabric.Finish();
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
   shin.Profile(new Vector3(0,0,-.012f),new[]{-.325f,-.29f,-.17f,-.04f,.015f},new[]{.072f,.077f,.084f,.095f,.083f},new[]{.076f,.081f,.091f,.099f,.077f},12,.76f);
   shin.Profile(new Vector3(0,0,-.012f),new[]{-.328f,-.307f,-.28f},new[]{.078f,.084f,.082f},new[]{.081f,.087f,.086f},12,.68f);shin.Finish();
   var footOrigin=new Vector3(0,-.36f,-.007f);
   var white=new RunnerMeshBuilder(knee,"WhiteSneakerLast",ivory);
   white.Shoe(footOrigin,-.13f,-.081f,0);white.Shoe(footOrigin,-.081f,.025f,.035f);
   for(int lace=0;lace<3;lace++){float z=.025f+lace*.040f;white.Stitch(footOrigin+new Vector3(-.064f,.026f-lace*.006f,z),footOrigin+new Vector3(.064f,.026f-lace*.006f,z),.006f);}white.Finish();
   var accent=new RunnerMeshBuilder(knee,"AmberSneakerPanels",amber);
   accent.Sweep(new[]{footOrigin+new Vector3(side*.082f,-.040f,-.09f),footOrigin+new Vector3(side*.106f,-.048f,.015f),footOrigin+new Vector3(side*.106f,-.046f,.105f)},new[]{.027f,.029f,.017f},new[]{.006f,.006f,.005f},6);
   accent.Profile(footOrigin+new Vector3(0,0,-.079f),new[]{-.075f,-.027f,.030f},new[]{.075f,.079f,.070f},new[]{.027f,.030f,.023f},10,.7f);accent.Finish();
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
   for(int finger=0;finger<3;finger++){float x=-.028f+finger*.027f;limb.Sweep(new[]{new Vector3(x,-.304f,.044f),new Vector3(x,-.329f,.056f),new Vector3(x,-.344f,.035f)},new[]{.013f,.014f,.010f},new[]{.011f,.012f,.009f},6);}limb.Finish();
   var cuffTrim=new RunnerMeshBuilder(arm,"AmberSleevePiping",amber);
   cuffTrim.Profile(Vector3.zero,new[]{-.263f,-.252f},new[]{.096f,.096f},new[]{.096f,.096f},12,.72f);cuffTrim.Finish();
   var band=new RunnerMeshBuilder(elbow,"WristBand",dark);band.Profile(Vector3.zero,new[]{-.245f,-.214f},new[]{.053f,.055f},new[]{.052f,.054f},12);band.Finish();
  }

  static void BuildHead(RunnerAnimator animator,Material skin,Material ivory,Material dark,Material hair)
  {
   animator.Head=Pivot(animator.Hips,"Head",new Vector3(0,.815f,.003f));animator.Head.localScale=new Vector3(.91f,.94f,.91f);
   var neck=new RunnerMeshBuilder(animator.Hips,"Neck",skin);neck.Profile(new Vector3(0,0,.008f),new[]{.56f,.65f,.68f},new[]{.071f,.068f,.073f},new[]{.071f,.068f,.073f},12);neck.Finish();
   var face=new RunnerMeshBuilder(animator.Head,"FaceSkin",skin);
   face.Profile(new Vector3(0,0,.015f),new[]{-.215f,-.175f,-.085f,.03f,.12f,.195f,.225f},new[]{.085f,.132f,.181f,.196f,.19f,.151f,.085f},new[]{.066f,.105f,.140f,.157f,.145f,.115f,.06f},16,.9f);face.Finish();
   var features=new RunnerMeshBuilder(animator.Head,"FaceFeatures",skin);
   features.Loft(new[]{new Vector3(0,-.082f,.173f),new Vector3(0,-.055f,.203f),new Vector3(0,-.01f,.178f),new Vector3(0,.014f,.165f)},new[]{new Vector2(.025f,.014f),new Vector2(.029f,.024f),new Vector2(.021f,.014f),new Vector2(.013f,.010f)},8);
   for(int side=-1;side<=1;side+=2)features.Pebble(new Vector3(side*.196f,-.028f,-.005f),new Vector3(.067f,.108f,.070f),10);features.Finish();
   var eyes=new RunnerMeshBuilder(animator.Head,"EyeWhites",ivory);var expression=new RunnerMeshBuilder(animator.Head,"EyesAndExpression",dark);
   for(int side=-1;side<=1;side+=2){eyes.Pebble(new Vector3(side*.079f,.021f,.164f),new Vector3(.059f,.071f,.019f),10);expression.Pebble(new Vector3(side*.079f,.018f,.178f),new Vector3(.027f,.042f,.011f),8);expression.Sweep(new[]{new Vector3(side*.043f,.081f,.164f),new Vector3(side*.074f,.089f,.163f),new Vector3(side*.109f,.080f,.153f)},new[]{.008f,.011f,.006f},new[]{.004f,.005f,.003f},6);}
   expression.Sweep(new[]{new Vector3(-.042f,-.128f,.145f),new Vector3(0,-.136f,.153f),new Vector3(.043f,-.128f,.145f)},new[]{.003f,.004f,.003f},new[]{.002f,.002f,.002f},5);eyes.Finish();expression.Finish();
   var crown=new RunnerMeshBuilder(animator.Head,"SweptChestnutHair",hair);crown.HairCrown();
   // Calmer side-swept ridges grow from one crown; locks rake back rather than
   // forming a radial collection of identical cone spikes.
   for(int lockIndex=0;lockIndex<7;lockIndex++){
    float x=-.145f+lockIndex*.044f,y=.165f+(1-Mathf.Abs(x)/.16f)*.05f;
    crown.Sweep(new[]{new Vector3(x,y,.044f),new Vector3(x+.032f,y+.052f,-.013f),new Vector3(x+.060f,y+.061f+(lockIndex%3)*.006f,-.083f)},new[]{.045f,.033f,.004f},new[]{.035f,.021f,.003f},5,true);
   }
   for(int fringe=0;fringe<4;fringe++){
    float x=-.115f+fringe*.065f;
    crown.Sweep(new[]{new Vector3(x-.033f,.16f,.103f),new Vector3(x,.13f,.14f),new Vector3(x+.037f,.085f+(fringe%2)*.016f,.154f)},new[]{.042f,.032f,.004f},new[]{.024f,.018f,.003f},5,true);
   }
   crown.Finish();
  }

  static Material Material(GameObject owner,string name,Color color,float gloss)
  {var material=new Material(Resources.Load<Material>("Materials/Surface")){name=name,color=color};material.SetFloat("_Glossiness",gloss);ArtLifetime.Own(owner,material);return material;}
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
