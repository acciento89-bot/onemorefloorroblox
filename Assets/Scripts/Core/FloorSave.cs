using System;
using UnityEngine;
namespace Kamilunavo.OneMoreFloor.Core
{
 public static class FloorSave
 {
  public static string Key{
   get{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    if(!string.IsNullOrEmpty(QaKey))return QaKey;
#endif
    return "kamilunavo.onemorefloor.profile.v1";
   }
  }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
  public static string QaKey;
#endif
  public static FloorProfile Parse(string json){try{var p=JsonUtility.FromJson<FloorProfile>(json);if(p==null||p.Schema!=1)return new FloorProfile();p.Normalize();return p;}catch{return new FloorProfile();}}
  public static bool Writable{get;private set;}=true;
  public static FloorProfile Load(){string raw=PlayerPrefs.GetString(Key,"");Writable=true;try{var header=JsonUtility.FromJson<FloorProfile>(raw);if(header!=null&&header.Schema!=1)Writable=false;}catch{}return Parse(raw);}
  public static void Save(FloorProfile p){if(!Writable)throw new InvalidOperationException("Newer save schema is preserved. Update the app before saving.");p.Normalize();PlayerPrefs.SetString(Key,JsonUtility.ToJson(p));PlayerPrefs.Save();}
 }
}
