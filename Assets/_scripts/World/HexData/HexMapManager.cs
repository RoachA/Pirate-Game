using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

//https://www.redblobgames.com/grids/hexagons/
public class HexMapManager : MonoBehaviour
{
    [Inject] private DiContainer _container;
    [Inject] private HexTypesContainer _hexTypesContainer;
    
    [SerializeField] private MapParams _mapParams;
    [SerializeField] private GameObject _hexPrefab;

    [Header("noise params")]
    [SerializeField] private float PerlinScale;
    [SerializeField] private float YScale;

    private List<GameObject> _hexObjList = new List<GameObject>();
    private Dictionary<HexData, HexView> _hexToHexViewMap = new Dictionary<HexData, HexView>();
    private Dictionary<Vector3, HexData> _posVectorToHexBaseMap = new Dictionary<Vector3, HexData>();

    //if the row is 0 or even south neighbors must offset +1x
    //else the north neighbors must offset -1x
    private readonly Dictionary<Directions, Vector3> _directionVectors = new Dictionary<Directions, Vector3>(6)
    {
        {Directions.W, new Vector3(-1, 0, 1)},
        {Directions.E, new Vector3(1, 0, -1)},
        {Directions.SW, new Vector3(-1, 1, 0)},
        {Directions.SE, new Vector3(0, 1, -1)},
        {Directions.NW, new Vector3(0, -1, 1)},
        {Directions.NE, new Vector3(1, -1, 0)},
    };
    
    public enum Directions
    {
        W,
        E,
        SW,
        SE,
        NW,
        NE,
    }
    
    //----------------------->
    private void Start()
    {
        GenerateBlankMap();
    }

    [Button]
    public void GenerateBlankMap()
    {
        //Generates a blank map of ocean.
        ResetMap();
        
        for (int column = 0; column < _mapParams.Column; column++)
        {
            for (int row = 0; row < _mapParams.Row; row++)
            {
                HexData hex = new HexData(column, row);
                var obj = Instantiate(_hexPrefab, hex.WorldPosition(), quaternion.identity, this.transform);
                obj.name = "Q" + column.ToString() + " R" + row.ToString() + " S" + hex.S;
                _hexObjList.Add(obj);
                var view = obj.GetComponent<HexView>();
                view.InitHexView(_hexTypesContainer.GetDefinitionData(HexSurfaceType.Ocean));
                _hexToHexViewMap.Add(hex, view);
                _posVectorToHexBaseMap.Add(new Vector3(hex.Q, hex.R, hex.S), hex);
                _container.Inject(view);
            }
        }

        SetupLandMass();
    }

    private List<MapResourceBase> GenerateYieldDataForHex(HexDefinitionData data)
    {
        var yieldsList = new List<MapResourceBase>();
        
        foreach (var yield in data.YieldsMap.Yields)
        {
            var resource = new MapResourceBase();
            resource.Type = yield.Type;
            resource.Value = UnityEngine.Random.Range(yield.Range.x, yield.Range.y);
            yieldsList.Add(resource);
        }

        return yieldsList;
    }
    
    //we will run various passes to determine how the map looks. first determine the land mass.

    /// <summary>
    /// uses a perlin noise filter to determine this.
    /// </summary>
    private void SetupLandMass()
    {
        var perlinMap = GeneratePerlinNoiseMap(PerlinScale);
        var noiseList = new List<float>();

        foreach (var val in perlinMap)
        {
            noiseList.Add(val);
        }

        var index = 0;
        foreach (var registeredHex in _hexToHexViewMap)
        {
            var height = noiseList[index];
            
            if (height > _mapParams.PlainsThreshold)
            {
                HexDefinitionData typeData = _hexTypesContainer.GetDefinitionData(HexSurfaceType.Plains); 
                registeredHex.Value.InitHexView(typeData);
                registeredHex.Key.SetResourceYield(GenerateYieldDataForHex(typeData));
                registeredHex.Value.transform.localScale += YScale * Vector3.up * noiseList[index];
            } 
            
            if (height > _mapParams.MountainsThreshold) 
            {
                var typeData = _hexTypesContainer.GetDefinitionData(HexSurfaceType.Mountain); 
                registeredHex.Value.InitHexView(typeData);
                registeredHex.Key.SetResourceYield(GenerateYieldDataForHex(typeData));
                registeredHex.Value.transform.localScale += YScale * Vector3.up * noiseList[index];
            }
            
            index++;
        }
    }
    
    private float[,] GeneratePerlinNoiseMap(float scale) 
    {
        float rnd = Random.Range(scale / 2f, scale);
        
        float[,] noiseMap = new float[_mapParams.Column, _mapParams.Row];

        for (int y = 0; y < _mapParams.Row; y++) 
        {
            for (int x = 0; x < _mapParams.Column; x++) 
            {
                float sampleX = x / rnd;
                float sampleY = y / rnd;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = noiseValue;
            }
        }

        return noiseMap;
    }

    [Button]
    public void ResetMap()
    {
        if (_hexObjList == null || _hexObjList.Count == 0) return;

        foreach (var obj in _hexObjList)
        {
            DestroyImmediate(obj);
        }
        
        _hexObjList.Clear();
        _hexToHexViewMap.Clear();
        _posVectorToHexBaseMap.Clear();
    }

    #region Helpers

    /// <summary>
    /// Return vector value of a given direction, vector value is needed to work with the mapped data.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Vector3 GetDirectionVector(Directions direction)
    {
        if (_directionVectors.TryGetValue(direction, out Vector3 directionVector))
        {
            return directionVector;
        }
        else
        {
            Debug.LogWarning("Direction " + direction + " not found in dictionary.");
            return Vector3.zero; 
        }
    }

    /// <summary>
    /// Provides the neighbor at direction if it exists.
    /// </summary>
    /// <param name="targetHex">whose neighbor</param>
    /// <param name="direction">which direction to check for neighbor</param>
    /// <param name="returnedHex">neighboring hex</param>
    /// <returns></returns>
    public bool GetNeighborHexAtDirection(HexData targetHex, Directions direction, out HexData returnedHex)
    {
        returnedHex = null;
        var directionVector = GetDirectionVector(direction);
      
        var targetHexID = new Vector3(
            targetHex.Q + directionVector.x, 
            targetHex.R + directionVector.y,
            targetHex.S + directionVector.z
            );

        //IMPORTANT -> fixes odd and even offsets while determining the neighbors. todo this must be the case for whole lot of operations it seems!
        if (targetHex.R % 2 == 0)
        {
            if (direction == Directions.SE || direction == Directions.SW) targetHexID += new Vector3(1, 0, -1);
        }
        else
        {
            if (direction == Directions.NW || direction == Directions.NE) targetHexID += new Vector3(-1, 0, +1);
        }
        
        if (_posVectorToHexBaseMap.TryGetValue(targetHexID, out var hex))
        {
            returnedHex = hex;
            return true;
        }
    
        return false;
    }
    
    public HexData GetHexBaseByView(HexView hexView)
    {
        return _hexToHexViewMap.FirstOrDefault(pair => pair.Value == hexView).Key;
    }

    public float GetHexDistance(HexData hexA, HexData hexB)
    {
        var vec = CubicSubtract(hexA, hexB);
        return (Mathf.Abs(vec.x) + Mathf.Abs(vec.y) + Mathf.Abs(vec.z)) / 2;
    }

    private Vector3 CubicSubtract(HexData a, HexData b)
    {
        return new Vector3(a.Q - b.Q, a.R - b.R, a.S - b.S);
    }

    #endregion

    private void OnDrawGizmos()
    {
        foreach (var obj in _hexObjList)
        {
            Handles.Label(obj.transform.position + Vector3.left * .25f, obj.name);
        }
    }
}

[Serializable]
public struct MapParams
{
    [Range(0, 100)]
    public int Column;
    [Range(0, 100)]
    public int Row;

    [Header("Terrain Params")]
    [Range(0, 1f)]
    public float PlainsThreshold;
    [Range(0, 1f)]
    public float MountainsThreshold;

    public MapParams(int column, int row, float plainsThreshold, float mountainsThreshold)
    {
        Row = row;
        Column = column;
        PlainsThreshold = plainsThreshold;
        MountainsThreshold = mountainsThreshold;
    }
}

/*function cube_to_axial(cube):
var q = cube.q
var r = cube.r
return Hex(q, r)

function axial_to_cube(hex):
var q = hex.q
var r = hex.r
var s = -q-r
return Cube(q, r, s)*/