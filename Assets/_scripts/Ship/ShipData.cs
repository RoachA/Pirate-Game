using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ship
{
    [Serializable]
    public class ShipData
    {
        public string ShipName;
        [Space(10)]
        [TextArea(5, 20)]
        public string ShipDescription;
        [InlineProperty]
        public ShipBaseStats Stats;
    }

    public enum ShipSize
    {
        Tiny,
        Small,
        Medium,
        Large,
    }

    [Serializable]
    public class ShipBaseStats
    {
        [Range(0, 10)]
        public int Prevalence;
        public ShipSize Size;
        public int Maneuverability;
        public int Durability;
        public int BaseSpeed;
        public int MaxCannons;
        public int MaxCrew;
        public int MinCrew;
        public int IdealCrew;
        public int Cargo;
        public int BasePrice;
        //Best Sailing Point, laters.
    }
}