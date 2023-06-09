using UnityEngine;
using static DataSakura.AA.Runtime.Battle.Airplane.PlaneView;

namespace DataSakura.AA.Runtime.Battle.Airplane
{
    public class LandingArea : MonoBehaviour
    {
        [SerializeField] private Runway runway;

        private void OnTriggerEnter(Collider other)
        {
            //Check if colliding object has airplane collider component
            if (other.transform.TryGetComponent<SimpleAirPlaneCollider>(out SimpleAirPlaneCollider _airPlaneCollider))
            {
                //Calculate that the plane is coming from the right direction
                Vector3 dirFromLandingAreaToPlayerPlane = (transform.position - _airPlaneCollider.transform.position).normalized;
                float _directionFloat = Vector3.Dot(transform.forward, dirFromLandingAreaToPlayerPlane);

                //If direction is right start landing
                if (_directionFloat > 0.5f)
                {
                    PlaneView view = _airPlaneCollider.view;

                    runway.landingAdjuster.position = view.transform.position;

                    runway.AddAirplane(view);
                    view.airplaneState = AirplaneState.Landing;
                    view.AddLandingRunway(runway);
                }
            }
        }
    }
}