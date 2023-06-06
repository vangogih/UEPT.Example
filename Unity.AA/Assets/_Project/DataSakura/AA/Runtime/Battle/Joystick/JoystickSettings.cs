using Sirenix.OdinInspector;
using UnityEngine;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    /// <summary>
    /// Настройи джойстика
    /// </summary>
    [CreateAssetMenu(menuName = "GameSettings/Joystick Settings", fileName = "JoystickSettings")]
    public sealed class JoystickSettings : ScriptableObject
    {
        [SerializeField]
        [Min(0.01f)]
        [InfoBox("Смещение джойстика, при котором он считается активным")]
        private float handleOffsetActivation = 0.14f;
        
        [SerializeField]
        [InfoBox("Количество смещений джойстика, при котором происходит перемещение")]
        private float joystickJitter = 0.5f;

        public float JoystickJitter => joystickJitter;
        public float HandleOffsetActivation => handleOffsetActivation;
    }
}