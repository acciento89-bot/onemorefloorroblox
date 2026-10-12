using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Gameplay
{
 // One vertical carriage approaches a fixed tower floor. Forecasts and live poses
 // use the same clock; the target balcony never inherits this transform.
 public sealed class TowerLiftMotion:MonoBehaviour
 {
  public const float FloorSpacing=4.6f,TowerX=3.35f,TowerZ=.8f,ExitFraction=.38f,JumpArc=.65f;
  int from,to,realm,seed;float start;bool held;
  public static float LandingWindow(int floor)=>floor<5?.30f:floor<15?.25f:.22f;
  public void BeginRide(int source,int target,int chapter,int routeSeed,float clock){from=source;to=target;realm=chapter;seed=routeSeed;start=clock;held=false;Tick(clock);}
  public bool Ready(float clock)=>!held&&clock-start>=1.2f;
  public void Hold(int floor){from=to=floor;held=true;Tick(start);}
  public Vector3 At(float clock){float elapsed=Mathf.Max(0,clock-start);float y;if(held)y=to*FloorSpacing;else if(elapsed<1.2f)y=Mathf.Lerp(from*FloorSpacing,to*FloorSpacing,Mathf.SmoothStep(0,1,elapsed/1.2f));else{float t=elapsed-1.2f,speed=.75f+realm*.12f+to*.007f;float wave=Mathf.Sin(t*speed);if(realm==1)wave=.8f*wave+.2f*Mathf.Sin(t*speed*2);if(realm==2)wave=Mathf.Sin(t*speed+.17f*Mathf.Sin(t*.8f));y=to*FloorSpacing+wave*(.95f+(seed%7)*.015f);}return new Vector3(0,y,0);}
  public void Tick(float clock)=>transform.position=At(clock);
 }
}
