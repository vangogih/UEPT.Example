using System.Collections.Generic;
using DataSakura.Runtime.Battle.Input;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace DataSakura.Runtime.Battle.Airplane
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlaneView : MonoBehaviour
    {
        public enum AirplaneState
        {
            Flying,
            Landing,
        }

        private List<SimpleAirPlaneCollider> _airPlaneColliders = new List<SimpleAirPlaneCollider>();

        public AirplaneState airplaneState;

        [Header("Refs")] public Camera planeCamera;
        public Transform firePivot;

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

        [Header("Debug")]
        [ReadOnly]
        [ShowInInspector]
        private float _maxSpeed = 0.6f;
        private float _currentYawSpeed;
        private float _currentPitchSpeed;
        private float _currentRollSpeed;
        private float _currentSpeed;
        private float _currentEngineLightIntensity;
        private float _currentEngineSoundPitch;

        private readonly BoolReactiveProperty _isDead = new(false);

        private Rigidbody _rb;

        private float _inputH;
        private float _inputV;
        private float _inputYaw;
        private bool _inputTurbo;

        private PlaneConfig _planeConfig;
        private IInput _input;
        private bool _isPlayer;
        public bool IsPlayer => _isPlayer;
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        public float RollAngle { get; private set; }
        public float PitchAngle { get; private set; }
        public float ForwardSpeed { get; private set; }

        private void HandleInputs()
        {
            var d = _input.Direction;
            _inputH = d.x * _planeConfig.Responsiveness;
            _inputV = d.y * _planeConfig.Responsiveness;
            _inputYaw = d.z * _planeConfig.Responsiveness;
        }

        public void Initialize(PlaneConfig planeConfig, IInput input, bool isPlayer)
        {
            {
                _maxSpeed = defaultSpeed;
                _currentSpeed = defaultSpeed;

                _rb = GetComponent<Rigidbody>();
                _rb.isKinematic = true;
                _rb.useGravity = false;
                _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }

            _isPlayer = isPlayer;
            _planeConfig = planeConfig;
            _input = input;

            if (isPlayer) {
                gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.PlayerBody); 
                ChangeWingTrailEffectThickness(trailThickness);
            }
            else {
                gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnemyBody);
                Destroy(planeCamera.gameObject);
            }
            
            SetupColliders(crashCollidersRoot);
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
            }
            
            CalculateRollAndPitchAngles();
            CalculateForwardSpeed();
        }

        #region Flying State

        private void FlyingUpdate()
        {
            UpdatePropellersAndLights();

            //Airplane move only if not dead
            if (!_isDead.Value) {
                Movement();
                SidewaysForceCalculation();
            }
            else {
                ChangeWingTrailEffectThickness(0f);
            }

            //Crash
            if (!_isDead.Value && HitSometing()) {
                Crash();
            }
        }

        private void SidewaysForceCalculation()
        {
            float mutiplierXRot = sidewaysMovement * sidewaysMovementXRot;
            float mutiplierYRot = sidewaysMovement * sidewaysMovementYRot;

            float mutiplierYPos = sidewaysMovement * sidewaysMovementYPos;
            Vector3 localEulerAngles = transform.localEulerAngles;

            //Right side 
            if (localEulerAngles.z > 270f && localEulerAngles.z < 360f) {
                float angle = (localEulerAngles.z - 270f) / (360f - 270f);
                float invert = 1f - angle;

                transform.Rotate(Vector3.up * (invert * mutiplierYRot * Time.deltaTime));
                transform.Rotate(Vector3.right * (-invert * mutiplierXRot * _currentPitchSpeed * Time.deltaTime));

                transform.Translate(transform.up * (invert * mutiplierYPos * Time.deltaTime));
            }

            //Left side
            if (localEulerAngles.z > 0f && localEulerAngles.z < 90f) {
                float angle = localEulerAngles.z / 90f;

                transform.Rotate(-Vector3.up * (angle * mutiplierYRot * Time.deltaTime));
                transform.Rotate(Vector3.right * (-angle * mutiplierXRot * _currentPitchSpeed * Time.deltaTime));

                transform.Translate(transform.up * (angle * mutiplierYPos * Time.deltaTime));
            }

            //Right side down
            if (localEulerAngles.z > 90f && localEulerAngles.z < 180f) {
                float angle = (localEulerAngles.z - 90f) / (180f - 90f);
                float invert = 1f - angle;

                transform.Translate(transform.up * (invert * mutiplierYPos * Time.deltaTime));
                transform.Rotate(Vector3.right * (-invert * mutiplierXRot * _currentPitchSpeed * Time.deltaTime));
            }

            //Left side down
            if (localEulerAngles.z > 180f && localEulerAngles.z < 270f) {
                float angle = (localEulerAngles.z - 180f) / (270f - 180f);

                transform.Translate(transform.up * (angle * mutiplierYPos * Time.deltaTime));
                transform.Rotate(Vector3.right * (-angle * mutiplierXRot * _currentPitchSpeed * Time.deltaTime));
            }
        }

        private void Movement()
        {
            //Move forward
            transform.Translate(Vector3.forward * (_currentSpeed * Time.deltaTime));

            //Rotate airplane by inputs
            transform.Rotate(Vector3.forward * (-_inputH * _currentRollSpeed * Time.deltaTime));
            transform.Rotate(Vector3.right * (_inputV * _currentPitchSpeed * Time.deltaTime));

            //Rotate yaw
            transform.Rotate(Vector3.up * (_inputYaw *_currentYawSpeed * Time.deltaTime));

                //Accelerate and deacclerate
            if (_currentSpeed < _maxSpeed) {
                _currentSpeed += accelerating * Time.deltaTime;
            }
            else {
                _currentSpeed -= deaccelerating * Time.deltaTime;
            }

            //Turbo
            if (_inputTurbo) {
                //Set speed to turbo speed and rotation to turbo values
                _maxSpeed = turboSpeed;

                _currentYawSpeed = yawSpeed * yawTurboMultiplier;
                _currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
                _currentRollSpeed = rollSpeed * rollTurboMultiplier;

                //Engine lights
                _currentEngineLightIntensity = turbineLightTurbo;

                //Effects
                ChangeWingTrailEffectThickness(trailThickness);

                //Audio
                _currentEngineSoundPitch = turboSoundPitch;
            }
            else {
                //Speed and rotation normal
                _maxSpeed = defaultSpeed;

                _currentYawSpeed = yawSpeed;
                _currentPitchSpeed = pitchSpeed;
                _currentRollSpeed = rollSpeed;

                //Engine lights
                _currentEngineLightIntensity = turbineLightDefault;

                //Effects
                // ChangeWingTrailEffectThickness(0f);

                //Audio
                _currentEngineSoundPitch = defaultSoundPitch;
            }
        }
        
        private void CalculateRollAndPitchAngles()
        {
            // Calculate roll & pitch angles
            // Calculate the flat forward direction (with no y component).
            var flatForward = transform.forward;
            flatForward.y = 0;
            // If the flat forward vector is non-zero (which would only happen if the plane was pointing exactly straight upwards)
            if (flatForward.sqrMagnitude > 0)
            {
                flatForward.Normalize();
                // calculate current pitch angle
                var localFlatForward = transform.InverseTransformDirection(flatForward);
                PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);
                // calculate current roll angle
                var flatRight = Vector3.Cross(Vector3.up, flatForward);
                var localFlatRight = transform.InverseTransformDirection(flatRight);
                RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            }
        }

        private void CalculateForwardSpeed()
        {
            // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
            var localVelocity = transform.InverseTransformDirection(_rb.velocity);
            ForwardSpeed = Mathf.Max(0, localVelocity.z);
        }

        #endregion

        #region Landing State

        //My trasform is runway landing adjuster child
        private void LandingUpdate()
        {
            UpdatePropellersAndLights();

            ChangeWingTrailEffectThickness(0f);

            //Stop speed
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime);

            //Set local rotation to zero
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), 2f * Time.deltaTime);
        }

        #endregion

        #region Audio

        private void AudioSystem()
        {
            if (engineSoundSource == null)
                return;

            if (airplaneState == AirplaneState.Flying) {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, _currentEngineSoundPitch, 10f * Time.deltaTime);

                if (_isDead.Value) {
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
        }

        #endregion

        #region Private methods

        private void UpdatePropellersAndLights()
        {
            if (!_isDead.Value) {
                //Rotate propellers if any
                if (propellers.Length > 0) {
                    RotatePropellers(propellers, _currentSpeed * propelSpeedMultiplier);
                }

                //Control lights if any
                if (turbineLights.Length > 0) {
                    ControlEngineLights(turbineLights, _currentEngineLightIntensity);
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

        private void SetupColliders(Transform root)
        {
            if (root == null)
                return;

            //Get colliders from root transform
            Collider[] colliders = root.GetComponentsInChildren<Collider>();

            //If there are colliders put components in them
            for (int i = 0; i < colliders.Length; i++) {
                //Change collider to trigger
                Collider _collider = colliders[i];
                _collider.isTrigger = true;
                _collider.gameObject.layer = gameObject.layer;

                GameObject currentObject = _collider.gameObject;

                //Add airplane collider to it and put it on the list
                SimpleAirPlaneCollider airplaneCollider = currentObject.AddComponent<SimpleAirPlaneCollider>();
                _airPlaneColliders.Add(airplaneCollider);

                //Add airplane conroller reference to collider
                airplaneCollider.view = this;

                //Add rigid body to it
                Rigidbody rb = currentObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }

        private void RotatePropellers(GameObject[] rotateThese, float speed)
        {
            for (int i = 0; i < rotateThese.Length; i++) {
                rotateThese[i].transform.Rotate(Vector3.forward * (-speed * Time.deltaTime));
            }
        }

        private void ControlEngineLights(Light[] lights, float intensity)
        {
            for (int i = 0; i < lights.Length; i++) {
                if (!_isDead.Value) {
                    lights[i].intensity = Mathf.Lerp(lights[i].intensity, intensity, 10f * Time.deltaTime);
                }
                else {
                    lights[i].intensity = Mathf.Lerp(lights[i].intensity, 0f, 10f * Time.deltaTime);
                }
            }
        }

        private void ChangeWingTrailEffectThickness(float thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++) {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, thickness, Time.deltaTime * 10f);
            }
        }

        private bool HitSometing()
        {
            for (int i = 0; i < _airPlaneColliders.Count; i++) {
                if (_airPlaneColliders[i].collideSometing) {
                    //Reset colliders
                    foreach (SimpleAirPlaneCollider airPlaneCollider in _airPlaneColliders) {
                        airPlaneCollider.collideSometing = false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void Crash()
        {
            //Set rigidbody to non cinematic
            _rb.isKinematic = false;
            _rb.useGravity = true;

            //Change every collider trigger state and remove rigidbodys
            for (int i = 0; i < _airPlaneColliders.Count; i++) {
                _airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
                Destroy(_airPlaneColliders[i].GetComponent<Rigidbody>());
            }

            //Kill player
            _isDead.Value = true;

            Destroy(gameObject, 5f);
        }

        #endregion

        #region Variables

        public void BuffMaxSpeed(float clamped01Percent)
        {
            _maxSpeed *= clamped01Percent;
        }

        public void ResetMaxSpeed()
        {
            _maxSpeed = defaultSpeed;
        }

        public void BuffDefaultSpeed(float clamped01Percent)
        {
            defaultSpeed *= clamped01Percent;
        }

        #endregion
    }
}