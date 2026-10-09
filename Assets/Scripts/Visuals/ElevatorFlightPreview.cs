using UnityEngine;
using Kamilunavo.OneMoreFloor.Gameplay;
namespace Kamilunavo.OneMoreFloor.Visuals
{
 public sealed class ElevatorFlightPreview:MonoBehaviour
 {
  public FloorCourse Course;private LineRenderer _line;
  private void Awake(){_line=gameObject.AddComponent<LineRenderer>();_line.positionCount=25;_line.useWorldSpace=true;_line.widthMultiplier=.085f;_line.numCapVertices=3;_line.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;var material=new Material(Shader.Find("Sprites/Default"));_line.sharedMaterial=material;ArtLifetime.Own(gameObject,material);}
  private void LateUpdate(){if(Course==null||Course.Player==null)return;_line.enabled=!Course.Paused&&!Course.IsTransferring&&!Course.Profile.RunBanked&&!Course.Profile.Completed;if(!_line.enabled)return;var a=Course.Player.position+Vector3.up*.06f;var b=Course.Destination+Vector3.up*.06f;for(int i=0;i<25;i++){float t=i/24f;_line.SetPosition(i,Vector3.Lerp(a,b,t)+Vector3.up*Mathf.Sin(t*Mathf.PI)*1.8f);}Color c=Course.PredictedHit==2?new Color(1,.79f,.15f,.8f):Course.PredictedHit==1?new Color(.12f,.8f,1,.7f):new Color(.95f,.32f,.25f,.7f);_line.startColor=c;_line.endColor=c;}
 }
}
