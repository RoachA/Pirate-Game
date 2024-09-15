using Game.Ship;
using TMPro;
using UnityEngine;

namespace Game.Debug
{
    public class DebugScreen : MonoBehaviour
    {
        [SerializeField] private Transform windIndicator;
        [SerializeField] private TextMeshProUGUI windVectorTxt;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI fps_txt;
        [SerializeField] private TextMeshProUGUI speed_txt;

        private float _deltaTime = 0.0f;
        private Rigidbody _playerRb;

        [Header("Game Connections")]
        [SerializeField] private ShipMovementController _ship;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            _playerRb = _ship.GetComponent<Rigidbody>();
        }

        void Update()
        {
            DebugWind();
            DebugFPS();
            DebugShip();
        }

        private void DebugFPS()
        {
            if (Application.targetFrameRate != 60)
                Application.targetFrameRate = 60;

            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            fps_txt.text = $"FPS: {fps:F1}";

            var fpsNormalized = UnityEngine.Mathf.InverseLerp(0, 30, fps);
            fps_txt.color = Color.Lerp(Color.red, Color.green, fpsNormalized);
        }

        private void DebugWind()
        {
            var windData = _ship.GetWindData();

            windIndicator.eulerAngles = new Vector3(0, 0, -windData.Direction);

            var kmH = windData.Power * 65.3f; // fake
            windVectorTxt.text = kmH.ToString("F2") + "km/h";
        }

        private void DebugShip()
        {
            var speed = Mathf.Clamp(_playerRb.velocity.magnitude, 0, 100);
            speed_txt.text = speed.ToString("F2") + "km/h";
        }
    }
}