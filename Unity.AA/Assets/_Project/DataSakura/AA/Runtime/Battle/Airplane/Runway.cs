using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.Airplane
{
    public class Runway : MonoBehaviour
    {
        private bool landingCompleted;
        private float landingSpeed;
        private PlaneView _landingAirplaneView;
        private Vector3 landingAdjusterStartLocalPos;

        [Header("Input")]
        [SerializeField] private KeyCode launchKey = KeyCode.Space;

        [Header("Runway references")]
        public string runwayName = "Runway";
        [SerializeField] private LandingArea landingArea;
        public Transform landingAdjuster;
        [SerializeField] private Transform landingfinalPos;

        private void Start()
        {
            landingSpeed = 1f;
            landingAdjusterStartLocalPos = landingAdjuster.localPosition;
        }

        private void Update()
        {
            //Airplane is landing (Landing area add airplane controller reference)
            if(_landingAirplaneView != null)
            {
                //Set airplane to landing adjuster child
                _landingAirplaneView.transform.SetParent(landingAdjuster.transform);

                //Move landing adjuster to landing final pos position
                if(!landingCompleted)
                {
                    landingSpeed += Time.deltaTime;
                    landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingfinalPos.localPosition, landingSpeed * Time.deltaTime);

                    float _distanceToLandingFinalPos = Vector3.Distance(landingAdjuster.position, landingfinalPos.position);
                    if (_distanceToLandingFinalPos < 0.1f)
                    {
                        landingCompleted = true;
                    }
                }
                else
                {
                    landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingfinalPos.localPosition, landingSpeed * Time.deltaTime);

                    //Launch airplane
                    if (Input.GetKeyDown(launchKey))
                    {
                        _landingAirplaneView.airplaneState = PlaneView.AirplaneState.Takeoff;
                    }

                    //Reset runway if landing airplane is taking off
                    if (_landingAirplaneView.airplaneState == PlaneView.AirplaneState.Flying)
                    {
                        _landingAirplaneView.transform.SetParent(null);
                        _landingAirplaneView = null;
                        landingCompleted = false;
                        landingSpeed = 1f;
                        landingAdjuster.localPosition = landingAdjusterStartLocalPos;
                    }
                }
            }
        }

        //Landing area add airplane controller reference
        public void AddAirplane(PlaneView plane)
        {
            _landingAirplaneView = plane;
        }

        public bool AirplaneLandingCompleted()
        {
            if (_landingAirplaneView != null)
            {
                if (_landingAirplaneView.airplaneState != PlaneView.AirplaneState.Takeoff)
                {
                    return landingCompleted;
                }
            }

            return false;
        }

        public bool AirplaneIsLanding()
        {
            if(_landingAirplaneView != null && !landingCompleted)
            {
                return true;
            }

            return false;
        }

        public bool AriplaneIsTakingOff()
        {
            if (_landingAirplaneView != null)
            {
                if(_landingAirplaneView.airplaneState == PlaneView.AirplaneState.Takeoff)
                {
                    return true;
                }
            }

            return false;
        }
    }
}