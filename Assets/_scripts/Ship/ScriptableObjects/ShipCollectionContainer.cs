using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Game.Ship
{
    [CreateAssetMenu(fileName = "ShipCollectionContainer", menuName = "DataContainers/ShipCollectionContainer", order = 1)]
    public class ShipCollectionContainer : ScriptableObject
    {
        [SerializeField] private List<ShipDataContainer> AvailableShips;

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
                    ShipDataContainer shipData = AssetDatabase.LoadAssetAtPath<ShipDataContainer>(filePath);
                    if (shipData != null)
                    {
                        AvailableShips.Add(shipData);
                        UnityEngine.Debug.Log($"Loaded ShipData: {shipData.name}");
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