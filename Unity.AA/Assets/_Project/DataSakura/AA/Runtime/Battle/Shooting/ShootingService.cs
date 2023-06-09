﻿using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Utilities;
using UnityEngine;

namespace DataSakura.AA.Runtime.Battle
{
    public class ShootingService : ILoadUnit
    {
        private readonly BattleConfigContainer _configContainer;
        private Bullet _bulletPrefab;

        public ShootingService(ConfigContainer configContainer)
        {
            _configContainer = configContainer.Battle;
        }

        public UniTask Load()
        {
            _bulletPrefab = AssetService.R.Load<Bullet>("Bullets/universal_bullet");
            return UniTask.CompletedTask;
        }

        public void Shoot(PlaneView planeView)
        {
            CreateBullet(planeView.firePivot, planeView.IsPlayer);
        }

        private Bullet CreateBullet(Transform spawnPoint, bool isPlayer)
        {
            var instance = Object.Instantiate(_bulletPrefab, spawnPoint.position, spawnPoint.rotation);

            BulletConfig bulletConfig = isPlayer ? _configContainer.PlaneConfig.Bullet : _configContainer.BotPlaneConfig.Bullet;

            int collideMask =
                LayerMask.NameToLayer(isPlayer ? RuntimeConstants.PhysicLayers.PlayerBullet : RuntimeConstants.PhysicLayers.EnemyBullet);

            instance.Initialize(bulletConfig, collideMask);
            return instance;
        }
    }
}