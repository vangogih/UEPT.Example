using UnityEngine;
using System.Collections.Generic;
using DataSakura.AA.Runtime;
using DataSakura.AA.Runtime.Battle.Joystick;

namespace HeneGames.Airplane
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlaneView : MonoBehaviour
    {
        public enum AirplaneState
        {
            Flying,
            Landing,
            Takeoff,
        }

        #region Private variables

        private List<SimpleAirPlaneCollider> airPlaneColliders = new List<SimpleAirPlaneCollider>();

        private float maxSpeed = 0.6f;
        private float currentYawSpeed;
        private float currentPitchSpeed;
        private float currentRollSpeed;
        private float currentSpeed;
        private float currentEngineLightIntensity;
        private float currentEngineSoundPitch;

        private bool planeIsDead;

        private Rigidbody rb;
        private Runway currentRunway;

        //Input variables
        private float inputH;
        private float inputV;
        private bool inputTurbo;
        private bool inputYawLeft;
        private bool inputYawRight;

        #endregion

        public AirplaneState airplaneState;

        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField]
        private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField]
        private float yawSpeed = 50f;

        [Range(5f, 500f)] [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)] [SerializeField] private float rollSpeed = 200f;

        [Header("Rotating speeds multiplers when turbo is used")]
        [Range(0.1f, 5f)]
        [SerializeField]
        private float yawTurboMultiplier = 0.3f;

        [Range(0.1f, 5f)] [SerializeField] private float pitchTurboMultiplier = 0.5f;

        [Range(0.1f, 5f)] [SerializeField] private float rollTurboMultiplier = 1f;

        [Header("Moving speed")]
        [Range(5f, 100f)]
        [SerializeField]
        private float defaultSpeed = 10f;

        [Range(10f, 200f)] [SerializeField] private float turboSpeed = 20f;

        [Range(0.1f, 50f)] [SerializeField] private float accelerating = 10f;

        [Range(0.1f, 50f)] [SerializeField] private float deaccelerating = 5f;

        [Header("Sideway force")]
        [Range(0.1f, 15f)]
        [SerializeField]
        private float sidewaysMovement = 15f;

        [Range(0.001f, 0.05f)]
        [SerializeField]
        private float sidewaysMovementXRot = 0.012f;

        [Range(0.1f, 5f)] [SerializeField] private float sidewaysMovementYRot = 1.5f;

        [Range(-1, 1f)] [SerializeField] private float sidewaysMovementYPos = 0.1f;

        [Header("Engine sound settings")]
        [SerializeField]
        private AudioSource engineSoundSource;

        [SerializeField] private float maxEngineSound = 1f;

        [SerializeField] private float defaultSoundPitch = 1f;

        [SerializeField] private float turboSoundPitch = 1.5f;

        [Header("Engine propellers settings")]
        [Range(10f, 10000f)]
        [SerializeField]
        private float propelSpeedMultiplier = 100f;

        [SerializeField] private GameObject[] propellers;

        [Header("Turbine light settings")]
        [Range(0.1f, 20f)]
        [SerializeField]
        private float turbineLightDefault = 1f;

        [Range(0.1f, 20f)] [SerializeField] private float turbineLightTurbo = 5f;

        [SerializeField] private Light[] turbineLights;

        [Header("Colliders")] [SerializeField] private Transform crashCollidersRoot;

        [Header("Takeoff settings")]
        [Tooltip("How far must the plane be from the runway before it can be controlled again")]
        [SerializeField]
        private float takeoffLenght = 30f;

        private PlaneConfig _planeConfig;
        private JoystickInput _joystickInput;

        private void Start()
        {
            //Setup speeds
            maxSpeed = defaultSpeed;
            currentSpeed = defaultSpeed;

            //Get and set rigidbody
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            SetupColliders(crashCollidersRoot);
        }

        private void HandleInputs()
        {
            //Rotate inputs
            var d = _joystickInput.Direction;
            inputH = -d.x * _planeConfig.Responsiveness; //Input.GetAxis("Horizontal");
            inputV = d.y * _planeConfig.Responsiveness;  //Input.GetAxis("Vertical");

            //Yaw axis inputs
            // inputYawLeft = Input.GetKey(KeyCode.Q);
            // inputYawRight = Input.GetKey(KeyCode.E);
            //
            // //Turbo
            // inputTurbo = Input.GetKey(KeyCode.LeftShift);
        }

        public void Initialize(PlaneConfig planeConfig, JoystickInput joystick)
        {
            _planeConfig = planeConfig;
            _joystickInput = joystick;
        }

        private void Update()
        {
            AudioSystem();
            HandleInputs();

            switch (airplaneState) {
                case AirplaneState.Flying:
                    FlyingUpdate();
                    break;

                case AirplaneState.Landing:
                    LandingUpdate();
                    break;

                case AirplaneState.Takeoff:
                    TakeoffUpdate();
                    break;
            }
        }

        #region Flying State

        private void FlyingUpdate()
        {
            UpdatePropellersAndLights();

            //Airplane move only if not dead
            if (!planeIsDead) {
                Movement();
                SidewaysForceCalculation();
            }
            else {
                ChangeWingTrailEffectThickness(0f);
            }

            //Crash
            if (!planeIsDead && HitSometing()) {
                Crash();
            }
        }

        private void SidewaysForceCalculation()
        {
            float _mutiplierXRot = sidewaysMovement * sidewaysMovementXRot;
            float _mutiplierYRot = sidewaysMovement * sidewaysMovementYRot;

            float _mutiplierYPos = sidewaysMovement * sidewaysMovementYPos;

            //Right side 
            if (transform.localEulerAngles.z > 270f && transform.localEulerAngles.z < 360f) {
                float _angle = (transform.localEulerAngles.z - 270f) / (360f - 270f);
                float _invert = 1f - _angle;

                transform.Rotate(Vector3.up * (_invert * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

                transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
            }

            //Left side
            if (transform.localEulerAngles.z > 0f && transform.localEulerAngles.z < 90f) {
                float _angle = transform.localEulerAngles.z / 90f;

                transform.Rotate(-Vector3.up * (_angle * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

                transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
            }

            //Right side down
            if (transform.localEulerAngles.z > 90f && transform.localEulerAngles.z < 180f) {
                float _angle = (transform.localEulerAngles.z - 90f) / (180f - 90f);
                float _invert = 1f - _angle;

                transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }

            //Left side down
            if (transform.localEulerAngles.z > 180f && transform.localEulerAngles.z < 270f) {
                float _angle = (transform.localEulerAngles.z - 180f) / (270f - 180f);

                transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }
        }

        private void Movement()
        {
            //Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            //Rotate airplane by inputs
            transform.Rotate(Vector3.forward * -inputH * currentRollSpeed * Time.deltaTime);
            transform.Rotate(Vector3.right * inputV * currentPitchSpeed * Time.deltaTime);

            //Rotate yaw
            if (inputYawRight) {
                transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
            }
            else if (inputYawLeft) {
                transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
            }

            //Accelerate and deacclerate
            if (currentSpeed < maxSpeed) {
                currentSpeed += accelerating * Time.deltaTime;
            }
            else {
                currentSpeed -= deaccelerating * Time.deltaTime;
            }

            //Turbo
            if (inputTurbo) {
                //Set speed to turbo speed and rotation to turbo values
                maxSpeed = turboSpeed;

                currentYawSpeed = yawSpeed * yawTurboMultiplier;
                currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
                currentRollSpeed = rollSpeed * rollTurboMultiplier;

                //Engine lights
                currentEngineLightIntensity = turbineLightTurbo;

                //Effects
                ChangeWingTrailEffectThickness(trailThickness);

                //Audio
                currentEngineSoundPitch = turboSoundPitch;
            }
            else {
                //Speed and rotation normal
                maxSpeed = defaultSpeed;

                currentYawSpeed = yawSpeed;
                currentPitchSpeed = pitchSpeed;
                currentRollSpeed = rollSpeed;

                //Engine lights
                currentEngineLightIntensity = turbineLightDefault;

                //Effects
                ChangeWingTrailEffectThickness(0f);

                //Audio
                currentEngineSoundPitch = defaultSoundPitch;
            }
        }

        #endregion

        #region Landing State

        public void AddLandingRunway(Runway _landingThisRunway)
        {
            currentRunway = _landingThisRunway;
        }

        //My trasform is runway landing adjuster child
        private void LandingUpdate()
        {
            UpdatePropellersAndLights();

            ChangeWingTrailEffectThickness(0f);

            //Stop speed
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);

            //Set local rotation to zero
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 2f * Time.deltaTime);
        }

        #endregion

        #region Takeoff State

        private void TakeoffUpdate()
        {
            UpdatePropellersAndLights();

            //Reset colliders
            foreach (SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders) {
                _airPlaneCollider.collideSometing = false;
            }

            //Accelerate
            if (currentSpeed < turboSpeed) {
                currentSpeed += (accelerating * 2f) * Time.deltaTime;
            }

            //Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            //Far enough from the runaway go back to flying state
            float _distanceToRunway = Vector3.Distance(transform.position, currentRunway.transform.position);

            if (_distanceToRunway > takeoffLenght) {
                currentRunway = null;
                airplaneState = AirplaneState.Flying;
            }
        }

        #endregion

        #region Audio

        private void AudioSystem()
        {
            if (engineSoundSource == null)
                return;

            if (airplaneState == AirplaneState.Flying) {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);

                if (planeIsDead) {
                    engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 10f * Time.deltaTime);
                }
                else {
                    engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
                }
            }
            else if (airplaneState == AirplaneState.Landing) {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, defaultSoundPitch, 1f * Time.deltaTime);
                engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 1f * Time.deltaTime);
            }
            else if (airplaneState == AirplaneState.Takeoff) {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, turboSoundPitch, 1f * Time.deltaTime);
                engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
            }
        }

        #endregion

        #region Private methods

        private void UpdatePropellersAndLights()
        {
            if (!planeIsDead) {
                //Rotate propellers if any
                if (propellers.Length > 0) {
                    RotatePropellers(propellers, currentSpeed * propelSpeedMultiplier);
                }

                //Control lights if any
                if (turbineLights.Length > 0) {
                    ControlEngineLights(turbineLights, currentEngineLightIntensity);
                }
            }
            else {
                //Rotate propellers if any
                if (propellers.Length > 0) {
                    RotatePropellers(propellers, 0f);
                }

                //Control lights if any
                if (turbineLights.Length > 0) {
                    ControlEngineLights(turbineLights, 0f);
                }
            }
        }

        private void SetupColliders(Transform _root)
        {
            if (_root == null)
                return;

            //Get colliders from root transform
            Collider[] colliders = _root.GetComponentsInChildren<Collider>();

            //If there are colliders put components in them
            for (int i = 0; i < colliders.Length; i++) {
                //Change collider to trigger
                colliders[i].isTrigger = true;

                GameObject _currentObject = colliders[i].gameObject;

                //Add airplane collider to it and put it on the list
                SimpleAirPlaneCollider _airplaneCollider = _currentObject.AddComponent<SimpleAirPlaneCollider>();
                airPlaneColliders.Add(_airplaneCollider);

                //Add airplane conroller reference to collider
                _airplaneCollider.view = this;

                //Add rigid body to it
                Rigidbody _rb = _currentObject.AddComponent<Rigidbody>();
                _rb.useGravity = false;
                _rb.isKinematic = true;
                _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }

        private void RotatePropellers(GameObject[] _rotateThese, float _speed)
        {
            for (int i = 0; i < _rotateThese.Length; i++) {
                _rotateThese[i].transform.Rotate(Vector3.forward * -_speed * Time.deltaTime);
            }
        }

        private void ControlEngineLights(Light[] _lights, float _intensity)
        {
            for (int i = 0; i < _lights.Length; i++) {
                if (!planeIsDead) {
                    _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, _intensity, 10f * Time.deltaTime);
                }
                else {
                    _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, 0f, 10f * Time.deltaTime);
                }
            }
        }

        private void ChangeWingTrailEffectThickness(float _thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++) {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
            }
        }

        private bool HitSometing()
        {
            for (int i = 0; i < airPlaneColliders.Count; i++) {
                if (airPlaneColliders[i].collideSometing) {
                    //Reset colliders
                    foreach (SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders) {
                        _airPlaneCollider.collideSometing = false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void Crash()
        {
            //Set rigidbody to non cinematic
            rb.isKinematic = false;
            rb.useGravity = true;

            //Change every collider trigger state and remove rigidbodys
            for (int i = 0; i < airPlaneColliders.Count; i++) {
                airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
                Destroy(airPlaneColliders[i].GetComponent<Rigidbody>());
            }

            //Kill player
            planeIsDead = true;

            //Here you can add your own code...
        }

        #endregion

        #region Variables

        /// <summary>
        /// Returns a percentage of how fast the current speed is from the maximum speed between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float PercentToMaxSpeed()
        {
            float _percentToMax = currentSpeed / turboSpeed;

            return _percentToMax;
        }

        public bool PlaneIsDead()
        {
            return planeIsDead;
        }

        public bool UsingTurbo()
        {
            if (maxSpeed == turboSpeed) {
                return true;
            }

            return false;
        }

        public float CurrentSpeed()
        {
            return currentSpeed;
        }

        #endregion
    }
}