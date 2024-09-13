using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _gameCam;
    public float moveSpeed = 5f;
    public float deceleration = 0.5f;

    private void Awake()
    {
        _gameCam = GetComponentInChildren<Camera>();
    }

    public Camera GetCamera()
    {
        return _gameCam;
    }
}
