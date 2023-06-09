using UnityEngine;
using UnityEngine.Serialization;

namespace DataSakura.AA.Runtime.Battle.Airplane
{
    public class SimpleAirPlaneCollider : MonoBehaviour
    {
        public bool collideSometing;

        [FormerlySerializedAs("controller")]
        [HideInInspector]
        public PlaneView view;

        private void OnTriggerEnter(Collider other)
        {
            //Collide someting bad
            if(other.gameObject.GetComponent<SimpleAirPlaneCollider>() == null && other.gameObject.GetComponent<LandingArea>() == null)
            {
                collideSometing = true;
            }
        }
    }
}