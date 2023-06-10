using DataSakura.AA.Runtime.Battle.Airplane;
using UnityEngine;

namespace _Project.DataSakura.AA.Editor.ManualTests
{
    public class TestFollowable : MonoBehaviour, IFollowable
    {
        public Transform Transform => transform;
        public float RollAngle { get; }
        public float PitchAngle { get; }
        public float ForwardSpeed { get; }
    }
}