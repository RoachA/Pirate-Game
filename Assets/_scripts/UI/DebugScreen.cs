using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Game.Player;
using TMPro;
using UnityEngine;

namespace Game.Debug
{
    public class DebugScreen : MonoBehaviour
    {
        [SerializeField] private Transform windIndicator;
        [SerializeField] private TextMeshProUGUI windVectorTxt;

        [Header("Game Connections")]
        [SerializeField] private PlayerMovementController player;

        void Update()
        {
            DebugWind();
        }

        private void DebugWind()
        {
            var windData = player.GetWindData();
            var dir = windData.Direction;
            float angleRadians = Mathf.Atan2(dir.y, dir.x);

            float angleDegrees = angleRadians * Mathf.Rad2Deg;
            windIndicator.eulerAngles = new Vector3(0, 0, angleDegrees);

            windVectorTxt.text = windData.Power.ToString(CultureInfo.InvariantCulture);
        }
    }
}