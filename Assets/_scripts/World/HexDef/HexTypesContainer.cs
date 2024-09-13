using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexTypesContainer", menuName = "LocalData/HexTypeContainer", order = 0)]
public class HexTypesContainer : ScriptableObject
{
   [SerializeField] public List<HexDefinitionData> HexTypesList;

   public HexDefinitionData GetDefinitionData(HexSurfaceType surfaceType)
   {
      HexDefinitionData data = null;
      
      foreach (var typeData in HexTypesList)
      {
         if (surfaceType == typeData.HexType) data = typeData;
      }

      if (data == null) Debug.LogError("no data was found for the requested type " + surfaceType);
      return data;
   }
}
