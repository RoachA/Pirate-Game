using UnityEngine;
using UniRx;
using Zenject;

public class InteractionManager : MonoBehaviour
{
    [Inject] private readonly SignalBus _bus;
    [Inject] private readonly CameraManager _cameraManager;

    private IInteractable _selectedHex;
    private IInteractable _targetedHex;

    private Camera _gameCam;
    private Vector3 _camVelocity; 
    
    private void Start()
    {
        _gameCam = _cameraManager.GetCamera();
    }
    
    void Update()
    {
        HandleHexInteractions();
        HandleCam();
    }
    
    private IInteractable _itemOnFocus;

    private void HandleSelect(IInteractable selectable)
    {
        _selectedHex = selectable;
        _bus.Fire(new CoreSignals.HexSelectedSignal(selectable));
    }

    private void HandleHover(IInteractable selectable)
    {
        _selectedHex = selectable;
        _bus.Fire(new CoreSignals.HexTargetedSignal(selectable));
    }

    private void HandleHexInteractions()
    {
        Ray ray = _gameCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            _targetedHex = null;
            _bus.Fire(new CoreSignals.HexTargetedSignal(null));
            return;
        }
        
        IInteractable selectable = hit.collider.GetComponent<IInteractable>();
        HandleHover(selectable);

        if (Input.GetMouseButtonDown(0))
        {
            if (selectable != null)
            {
                HandleSelect(selectable);
            }
        }  
    }

    private void HandleCam()
    {
        // Get the horizontal and vertical input from arrow keys
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        _camVelocity += movement * (_cameraManager.moveSpeed * Time.deltaTime);


        // Move the object based on the input
        _cameraManager.transform.Translate(_camVelocity * Time.deltaTime);
        _camVelocity -= _camVelocity * (_cameraManager.deceleration * Time.deltaTime);

    }
}
