using UnityEngine;

namespace Kamilunavo.OneMoreFloor.Gameplay
{
    public sealed class MovingPlatform:MonoBehaviour
    {
        public Vector3 Axis=Vector3.right; public float Distance=2f; public float Speed=1.2f;
        private Vector3 _origin; private float _phase;
        private void Awake(){_origin=transform.position;_phase=transform.GetSiblingIndex()*.61f;}
        private void Update(){transform.position=_origin+Axis*Mathf.Sin(Time.time*Speed+_phase)*Distance;}
    }
}
