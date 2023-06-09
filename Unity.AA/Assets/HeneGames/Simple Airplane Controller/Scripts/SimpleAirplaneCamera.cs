using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DataSakura.AA.Runtime.Battle.Airplane;
using UnityEngine.Serialization;

namespace HeneGames.Airplane
{
    public class SimpleAirplaneCamera : MonoBehaviour
    {
        [FormerlySerializedAs("airPlaneController")]
        [Header("References")]
        [SerializeField] private PlaneView airPlaneView;
        [SerializeField] private CinemachineFreeLook freeLook;
        [Header("Camera values")]
        [SerializeField] private float cameraDefaultFov = 60f;
        [SerializeField] private float cameraTurboFov = 40f;

        private void Start()
        {
            //Lock and hide mouse
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }

        // private void Update()
        // {
        //     CameraFovUpdate();
        // }
        //
        // private void CameraFovUpdate()
        // {
        //     //Turbo
        //     if(!airPlaneController.PlaneIsDead())
        //     {
        //         if (Input.GetKey(KeyCode.LeftShift))
        //         {
        //             ChangeCameraFov(cameraTurboFov);
        //         }
        //         else
        //         {
        //             ChangeCameraFov(cameraDefaultFov);
        //         }
        //     }
        // }
        //
        // public void ChangeCameraFov(float _fov)
        // {
        //     float _deltatime = Time.deltaTime * 100f;
        //     freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, _fov, 0.05f * _deltatime);
        // }
    }
}