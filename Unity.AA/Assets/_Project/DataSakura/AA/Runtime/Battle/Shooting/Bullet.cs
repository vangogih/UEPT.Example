using UnityEngine;

namespace DataSakura.AA.Runtime.Battle
{
    public sealed class Bullet : MonoBehaviour
    {
        private BulletConfig _config;
        private LayerMask _collideMask;

        public void Initialize(BulletConfig config, LayerMask collideMask)
        {
            _config = config;
            gameObject.layer = collideMask;
            Destroy(gameObject, config.LifeTime);
        }
        
        private void Update()
        {
            transform.position += transform.forward * (_config.Speed * Time.deltaTime);
        }
    }
}