using DataSakura.AA.Runtime.Battle.Airplane;
using UnityEngine;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    public sealed class BotBrain : IInput
    {
        public Vector2 Direction { get; private set; }
        public bool IsPressed { get; } = false;

        private readonly ShootingService _shootingService;
        private readonly BotPlaneConfig _botConfig;
        private readonly IFollowable _self;
        private readonly IFollowable _target;

        private float _sqrDistanceToShoot;

        public BotBrain(ShootingService shootingService,
            BotPlaneConfig botConfig,
            IFollowable self,
            IFollowable target)
        {
            _shootingService = shootingService;
            _botConfig = botConfig;
            _self = self;
            _target = target;

            _sqrDistanceToShoot = botConfig.DistanceToShoot * botConfig.DistanceToShoot;
        }

        public void FixedTick()
        {
            if (_target == null)
                return;

            Vector3 targetPosition = _target.Transform.position;
            Vector3 selfPosition = _self.Transform.position;
            Vector3 direction = targetPosition - selfPosition;

            Direction = new Vector2(direction.x, direction.z).normalized;
        }
    }
}