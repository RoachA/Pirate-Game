using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ship
{
    [CreateAssetMenu(fileName = "DefaultShipData", menuName = "DataContainers/ShipData", order = 1)]
    public class ShipDataContainer : ScriptableObject
    {
        public ShipData ShipDefinition;
    }
}