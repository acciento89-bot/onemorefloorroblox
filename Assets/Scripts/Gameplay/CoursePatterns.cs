using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Gameplay
{
 public static class CoursePatterns
 {
  public static Vector3[] Points(int realm,int seed){var random=new System.Random(seed);var points=new Vector3[30];for(int i=1;i<30;i++){float x=realm==0?Mathf.Sin(i*.7f)*1.5f:realm==1?Mathf.Sin(i*.9f)*2.2f:Mathf.Sin(i*.75f)*2.8f;points[i]=new Vector3(x,points[i-1].y+.8f+(float)random.NextDouble()*.20f,points[i-1].z+3.8f+(float)random.NextDouble()*.20f);}return points;}
  public static bool Moves(int index)=>index>=6&&(index-6)%5==0&&index<29;
 }
}
