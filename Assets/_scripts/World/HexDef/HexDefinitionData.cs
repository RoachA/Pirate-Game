using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HexType", menuName = "LocalData/HexTypeObject", order = 0)]
public class HexDefinitionData : ScriptableObject
{
    [SerializeField] public HexSurfaceType HexType;
    [SerializeField] public string HexName;
    [SerializeField] public HexTypeViewParams ViewParams;
    [SerializeField] public HexResourceYieldData YieldsMap;
}

[Serializable]
public struct HexTypeViewParams
{
    public Mesh HexBaseMesh;
    public Material HexBaseMaterial;
    public Color HexBaseColor;
    public bool IsPassable;
    public Sprite HexSprite;
}

[Serializable]
public enum HexSurfaceType
{
    Ocean = 0,
    Sea = 1,
    Plains = 2,
    Mountain = 3,
    Forest = 4,
    Swamp = 5,
    Desert = 6,
}
