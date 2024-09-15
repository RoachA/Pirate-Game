using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Ship
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private ShipTypeDataContainer _typeDataContainer;

        [ShowInInspector] [ReadOnly]
        public ShipTypeData ShipTypeData { private set; get; }
        public GameObject ShipBody; // will instantiate later.

        private void Awake()
        {
            InitializeShip();
        }

        private void InitializeShip()
        {
            ShipTypeData = new ShipTypeData
            {
                Stats = _typeDataContainer._shipTypeDefinition.Stats,
                ShipName = _typeDataContainer._shipTypeDefinition.ShipName,
                ShipDescription = _typeDataContainer._shipTypeDefinition.ShipDescription
            };
        }
    }
}