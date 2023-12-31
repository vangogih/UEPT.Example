using UnityEngine;
using UnityEngine.Serialization;

namespace DataSakura.Runtime.Battle.Airplane
{
    public class SimpleAirPlaneCollider : MonoBehaviour
    {
        public bool collideSometing;

        [FormerlySerializedAs("controller")]
        [HideInInspector]
        public PlaneView view;

        private void OnTriggerEnter(Collider other)
        {
            collideSometing = true;
        }
    }
}