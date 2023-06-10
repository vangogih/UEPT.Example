using DataSakura.AA.Runtime.Battle.Airplane;
using UniRx;
using UnityEngine;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    public sealed class BotBrain : IInput
    {
        public Vector3 Direction { get; private set; }
        public bool IsPressed { get; } = false;

        private readonly ShootingService _shootingService;
        private readonly BotPlaneConfig _config;
        private readonly PlaneView _self;
        private readonly PlaneView _target;

        private readonly float _sqrDistanceToShoot;
        private float _lastShootTime;
        private readonly float _randomPerlin;

        public BotBrain(ShootingService shootingService,
            BotPlaneConfig config,
            PlaneView self,
            PlaneView target)
        {
            _shootingService = shootingService;
            _config = config;
            _self = self;
            _target = target;

            _sqrDistanceToShoot = config.DistanceToShoot * config.DistanceToShoot;
            _randomPerlin = Random.Range(0f, 100f);

            Observable.EveryFixedUpdate().RxSubscribe(FixedTick);
        }

        public void FixedTick(long _)
        {
            if (_target == null)
                return;

            if (_self.IsDead.Value)
                return;

            SetDirection();

            Vector3 direction = _target.transform.position - _self.transform.position;

            if (direction.sqrMagnitude < _sqrDistanceToShoot && Time.time - _lastShootTime > _config.ShootInterval) {
                _shootingService.Shoot(_self);
                _lastShootTime = Time.time;
            }
        }

        /// <summary>
        /// based on: https://github.com/fredsa/unity-1st-person-racing/blob/master/Assets/Standard%20Assets/Vehicles/Aircraft/Scripts/AeroplaneAiControl.cs
        /// </summary>
        private void SetDirection()
        {
            // make the plane wander from the path, useful for making the AI seem more human, less robotic.
            Transform target = _target.transform;
            Transform transform = _self.transform;

            Vector3 targetPos = target.position +
                transform.right *
                ((Mathf.PerlinNoise(Time.time * _config.LateralWanderSpeed, _randomPerlin) * 2 - 1) * _config.LateralWanderDistance);

            // adjust the yaw and pitch towards the target
            Vector3 localTarget = transform.InverseTransformPoint(targetPos);
            float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
            float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);

            // Set the target for the planes pitch, we check later that this has not passed the maximum threshold
            targetAnglePitch = Mathf.Clamp(targetAnglePitch,
                -_config.MaxClimbAngle * Mathf.Deg2Rad,
                _config.MaxClimbAngle * Mathf.Deg2Rad);

            // calculate the difference between current pitch and desired pitch
            float changePitch = targetAnglePitch - _self.PitchAngle;

            // AI applies elevator control (pitch, rotation around x) to reach the target angle
            float pitchInput = changePitch * _config.PitchSensitivity;

            // clamp the planes roll
            float desiredRoll = Mathf.Clamp(targetAngleYaw, -_config.MaxRollAngle * Mathf.Deg2Rad, _config.MaxRollAngle * Mathf.Deg2Rad);
            float yawInput = 0;
            float rollInput = 0;

            yawInput = targetAngleYaw;
            rollInput = -(_self.RollAngle - desiredRoll) * _config.RollSensitivity;

            // adjust how fast the AI is changing the controls based on the speed. Faster speed = faster on the controls.
            float currentSpeedEffect = 1 + _self.ForwardSpeed * _config.SpeedEffect;
            rollInput *= currentSpeedEffect;
            pitchInput *= currentSpeedEffect;
            yawInput *= currentSpeedEffect;

            Direction = new Vector3(Mathf.Clamp(rollInput, -1, 1), Mathf.Clamp(pitchInput, -1, 1), Mathf.Clamp(yawInput, -1, 1));
        }
    }
}