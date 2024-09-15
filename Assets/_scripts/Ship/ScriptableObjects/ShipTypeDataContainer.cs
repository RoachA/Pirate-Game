using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ship
{
    [CreateAssetMenu(fileName = "DefaultShipData", menuName = "DataContainers/ShipTypeData", order = 1)]
    public class ShipTypeDataContainer : ScriptableObject
    {
        public ShipTypeData _shipTypeDefinition;
    }
}