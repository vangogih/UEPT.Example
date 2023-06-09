using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HeneGames.Airplane;

namespace HeneGames.Airplane
{
    public class RunwayUIManager : MonoBehaviour
    {
        [SerializeField] private Runway runway;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private GameObject uiContent;

        private void Update()
        {
            if(runway.AirplaneIsLanding())
            {
                uiContent.SetActive(true);
                debugText.text = "Airplane is landing";
            }
            else if(runway.AirplaneLandingCompleted())
            {
                uiContent.SetActive(true);
                debugText.text = "Press space to launch";
            }
            else if(runway.AriplaneIsTakingOff())
            {
                uiContent.SetActive(true);
                debugText.text = "Airplane is taking off";
            }
            else
            {
                uiContent.SetActive(false);
                debugText.text = "";
            }
        }
    }
}