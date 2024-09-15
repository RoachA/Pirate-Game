using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Game.Ship
{
    [CreateAssetMenu(fileName = "ShipTypeCollectionContainer", menuName = "DataContainers/ShipTypeCollectionContainer", order = 1)]
    public class ShipTypeCollectionContainer : ScriptableObject
    {
        [SerializeField] private List<ShipTypeDataContainer> AvailableShips;

        [Space(15)]
        [ShowInInspector] [ReadOnly]
        private readonly string _shipDataPath = "Assets/_scripts/Resources/Ship/";

        [Button]
        private void LoadShipDataFiles()
        {
            //
            AvailableShips.Clear();

            if (Directory.Exists(_shipDataPath))
            {
                string[] filePaths = Directory.GetFiles(_shipDataPath, "*.asset", SearchOption.AllDirectories);

                foreach (string filePath in filePaths)
                {
                    ShipTypeDataContainer shipTypeData = AssetDatabase.LoadAssetAtPath<ShipTypeDataContainer>(filePath);
                    if (shipTypeData != null)
                    {
                        AvailableShips.Add(shipTypeData);
                        UnityEngine.Debug.Log($"Loaded ShipTypeData: {shipTypeData.name}");
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Directory does not exist: {_shipDataPath}");
            }
        }
    }
}