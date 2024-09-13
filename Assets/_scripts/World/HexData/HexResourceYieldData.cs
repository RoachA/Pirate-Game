using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HexResourceYieldData
{
    [SerializeField] public List<ResourceYield> Yields;

    public HexResourceYieldData(List<ResourceYield> data)
    {
        Yields = data;
    }
}

[Serializable]
public struct ResourceYield
{
    public MapResourceType Type;
    public Vector2 Range;

    public ResourceYield(MapResourceType type, Vector2 range)
    {
        Range = range;
        Type = type;
    }
}
