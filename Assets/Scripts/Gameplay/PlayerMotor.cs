using UnityEngine;
using Kamilunavo.OneMoreFloor.Input;

namespace Kamilunavo.OneMoreFloor.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor:MonoBehaviour
    {
        public VirtualJoystick Joystick;public PressButton Jump;public Transform CameraTransform;public FloorCourse Course;public float Speed=6.2f;public float JumpSpeed=9.2f;public float Gravity=22f;private CharacterController _cc;private float _vertical;private bool _settling;private StepMarker _support;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private int _resetProbe;
#endif
        private void Awake()=>_cc=GetComponent<CharacterController>();
        public bool Paused;public Vector3 Velocity=>_cc!=null?_cc.velocity:Vector3.zero;public bool Grounded=>_cc!=null&&_cc.isGrounded;public event System.Action Jumped;
        public void ResetInput(){Joystick?.ResetInput();Jump?.ResetInput();}
        public void ResetMotion(){_support=null;_settling=true;_vertical=0;ResetInput();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        _resetProbe=8;
#endif
        }
        private void Update(){if(Paused){ResetInput();return;}
        // Route construction can consume a long frame. Establish ground contact before
        // accepting movement, rather than applying that loading time as player input.
        if(_settling){_settling=false;_cc.Move(Vector3.down*.1f);return;}
        var carry=Vector3.zero;if(_cc.isGrounded&&_support!=null){var moving=_support.transform.parent.GetComponent<MovingPlatform>();if(moving!=null)carry=moving.Delta;}
        var touch=Joystick!=null?Joystick.Value:Vector2.zero;var keys=new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"),UnityEngine.Input.GetAxisRaw("Vertical"));var input=touch.sqrMagnitude>.01f?touch:Vector2.ClampMagnitude(keys,1);var f=CameraTransform!=null?CameraTransform.forward:Vector3.forward;var r=CameraTransform!=null?CameraTransform.right:Vector3.right;f.y=0;r.y=0;f.Normalize();r.Normalize();var planar=f*input.y+r*input.x;if(planar.sqrMagnitude>.001f)transform.forward=Vector3.Slerp(transform.forward,planar.normalized,1-Mathf.Exp(-14*Time.deltaTime));if(_cc.isGrounded&&_vertical<0)_vertical=-2;var jump=UnityEngine.Input.GetKeyDown(KeyCode.Space)||(Jump!=null&&Jump.Consume());if(_cc.isGrounded&&jump){_support=null;_vertical=JumpSpeed;Jumped?.Invoke();}_vertical-=Gravity*Time.deltaTime;var v=planar*Speed;v.y=_vertical;_cc.Move(v*Time.deltaTime+carry);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if(_resetProbe-->0){bool floor=Physics.Raycast(transform.position+Vector3.up,Vector3.down,out var floorHit,3);Debug.Log("FLOOR_START_PROBE frame="+Time.frameCount+" dt="+Time.deltaTime+" pos="+transform.position+" grounded="+_cc.isGrounded+" velocity="+v+" floor="+floor+" floorY="+(floor?floorHit.point.y:0));}
#endif
        }
        public static bool IsLandingSupport(float normalY,float footY,float topY)=>normalY>=.45f&&footY>=topY-.08f;
        private void OnControllerColliderHit(ControllerColliderHit hit){
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        var probe=hit.collider.GetComponent<StepMarker>();if(probe!=null&&Course!=null&&probe.Index==Course.Height+1&&hit.normal.y>=.45f)Debug.Log("FLOOR_CONTACT_PROBE step="+probe.Index+" foot="+transform.position.y+" top="+hit.collider.bounds.max.y+" point="+hit.point+" normal="+hit.normal+" vertical="+_vertical+" dt="+Time.deltaTime+" accepted="+IsLandingSupport(hit.normal.y,transform.position.y,hit.collider.bounds.max.y));
#endif
        if(!IsLandingSupport(hit.normal.y,transform.position.y,hit.collider.bounds.max.y))return;var step=hit.collider.GetComponent<StepMarker>();if(step!=null){_support=step;Course?.Land(step);}}
    }
}
