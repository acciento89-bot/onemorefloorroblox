using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.OneMoreFloor.Gameplay
{
    public sealed class FloorCourse:MonoBehaviour
    {
        public Transform Player; public Text StageText; public Text ScoreText; public Text CoinsText; public Image Progress; public RectTransform CheckpointPanel; public Text CheckpointTitle;
        private readonly List<StagePlatform> _stages=new();
        private int _stage; private int _checkpoint; private int _score; private int _coins; private Vector3 _checkpointPosition;
        private static readonly Color Graphite=new(.06f,.07f,.10f); private static readonly Color Gold=new(1f,.57f,.08f); private static readonly Color Cyan=new(.06f,.65f,.95f);

        public void Build(){Random.InitState(260906);var x=0f;var y=0f;var z=0f;for(var i=0;i<30;i++){if(i>0){x=Mathf.Clamp(x+Random.Range(-2.4f,2.4f),-6f,6f);y+=Random.Range(.7f,1.05f);z+=Random.Range(4.3f,5.2f);}var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=$"Stage_{i+1:00}";go.transform.SetParent(transform,false);go.transform.position=new Vector3(x,y,z);go.transform.localScale=new Vector3(5.4f,.65f,4.2f);go.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=Graphite};var marker=go.AddComponent<StagePlatform>();marker.StageIndex=i;marker.IsCheckpoint=i>0&&(i+1)%5==0;_stages.Add(marker);AddEdge(go.transform,marker.IsCheckpoint?Cyan:Gold);if(i>1&&i%4==2){var move=go.AddComponent<MovingPlatform>();move.Distance=1.35f+Mathf.Min(1.25f,i*.03f);move.Speed=.8f+Mathf.Min(.8f,i*.02f);}}_checkpointPosition=_stages[0].transform.position+Vector3.up*1.5f;Player.position=_checkpointPosition;Refresh();BuildSkyline();}

        private static void AddEdge(Transform parent,Color color){var edge=GameObject.CreatePrimitive(PrimitiveType.Cube);edge.name="EdgeLight";edge.transform.SetParent(parent,false);edge.transform.localPosition=new Vector3(0,.56f,0);edge.transform.localScale=new Vector3(1.03f,.06f,1.03f);Destroy(edge.GetComponent<Collider>());edge.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=color};}

        private static void BuildSkyline(){var mat=new Material(Shader.Find("Standard")){color=new Color(.03f,.035f,.07f)};for(var i=0;i<22;i++){var b=GameObject.CreatePrimitive(PrimitiveType.Cube);b.name="City_Backdrop";var side=i%2==0?-1:1;var z=12+i*8f;b.transform.position=new Vector3(side*(10+Random.Range(0,18)),Random.Range(-6,5),z);b.transform.localScale=new Vector3(Random.Range(4,8),Random.Range(10,28),Random.Range(4,8));b.GetComponent<Renderer>().material=mat;Destroy(b.GetComponent<Collider>());}}

        private void Update(){if(Player==null||_stages.Count==0)return;var floorY=_stages[Mathf.Clamp(_stage,0,_stages.Count-1)].transform.position.y;if(Player.position.y<floorY-8f)RespawnCheckpoint();}

        public void Reach(StagePlatform p){if(p.StageIndex<=_stage||p.StageIndex>_stage+1)return;_stage=p.StageIndex;_score+=100+_stage*10;_coins+=1+_stage/5;if(p.IsCheckpoint){_checkpoint=_stage;_checkpointPosition=p.transform.position+Vector3.up*1.5f;ShowCheckpoint();}Refresh();}

        public void Continue(){if(CheckpointPanel!=null)CheckpointPanel.gameObject.SetActive(false);}
        public void RetryCheckpoint(){if(CheckpointPanel!=null)CheckpointPanel.gameObject.SetActive(false);RespawnCheckpoint();}

        public void RespawnCheckpoint(){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;Player.position=_checkpointPosition;if(cc!=null)cc.enabled=true;_stage=_checkpoint;Refresh();}

        private void ShowCheckpoint(){if(CheckpointPanel==null)return;CheckpointTitle.text=$"CHECKPOINT {_checkpoint+1} REACHED!";CheckpointPanel.gameObject.SetActive(true);}

        private void Refresh(){StageText.text=$"STAGE {_stage+1}/30";ScoreText.text=$"SCORE\n{_score:N0}";CoinsText.text=$"COINS\n{_coins}";if(Progress!=null)Progress.fillAmount=(_stage+1)/30f;}
    }
}
