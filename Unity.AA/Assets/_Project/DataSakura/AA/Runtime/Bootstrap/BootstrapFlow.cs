using UnityEngine;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        public void Start()
        {
            Debug.Log("Hello, World!");
        }
    }
}