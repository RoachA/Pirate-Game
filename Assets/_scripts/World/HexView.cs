using System;
using UnityEngine;
using Zenject;

public class HexView : MonoBehaviour, IInteractable
{
    [Inject] private readonly HexMapManager _hexMapManager;
    [Inject] private readonly SignalBus _bus;
     
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private MeshFilter _meshFilter;
    
    [Header("Debug")]
    [SerializeField] private HexMapManager.Directions _direction;
    [SerializeField] private float _gizmoRadius = 0.25f;
    [SerializeField] private Vector3 _gizmoOffset;

    private Vector3 _initScale;

    private void Start()
    {
        _bus.Subscribe<CoreSignals.HexSelectedSignal>(OnSelected);
        _bus.Subscribe<CoreSignals.HexTargetedSignal>(OnInFocus);
    }

    private void OnDestroy()
    {
        _bus.TryUnsubscribe<CoreSignals.HexSelectedSignal>(OnSelected);
        _bus.TryUnsubscribe<CoreSignals.HexTargetedSignal>(OnInFocus);
    }

    public void InitHexView(HexDefinitionData data)
    {
        _renderer.sharedMaterial = data.ViewParams.HexBaseMaterial;
        _initScale = _renderer.transform.localScale;
    }
    
    public void OnSelected(CoreSignals.HexSelectedSignal signal) //this will not carry hexData, this will reach hexmanager, get hex data and then will let the requesting element know.
    {
        if (signal.SelectedHex != this) return;

        var hexData = _hexMapManager.GetHexBaseByView(this);
        
        foreach (var resource in hexData.AvailableResources)
        {
            if (resource == null) return;
            Debug.Log(gameObject.name + " " + resource.Type + " " + resource.Value);
        }
    }

    public void OnInFocus(CoreSignals.HexTargetedSignal signal)
    {
        _renderer.transform.localScale = signal.TargetedHex == this ? _initScale * 0.9f : _initScale;
        if (signal.TargetedHex != this) return;
    }

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        var hex = _hexMapManager.GetHexBaseByView(this);
        HexData nachbar = null;
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.E, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.E ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.W, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.W ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.NW, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.NW ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.NE, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.NE ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.SE, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.SE ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
        
        if (_hexMapManager.GetNeighborHexAtDirection(hex, HexMapManager.Directions.SW, out nachbar))
        {
            Gizmos.color = _direction == HexMapManager.Directions.SW ? Color.green : Color.red;
            Gizmos.DrawSphere(nachbar.WorldPosition() + _gizmoOffset, _gizmoRadius);
        }
    }
    
    #endregion
}