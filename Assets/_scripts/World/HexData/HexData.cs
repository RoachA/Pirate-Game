using System.Collections.Generic;
using UnityEngine;

public class HexData
{
   public readonly int Q;
   public readonly int R;
   public readonly int S;

   static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
   
   public bool IsWalkable;
   public List<MapResourceBase> AvailableResources;
   
   public HexData(int q, int r)
   {
      //this.Q = q + (r + (r & 1)) / 2;
      this.Q = q;
      this.R = r;
      this.S = -(Q + R);
   }
   
//return world-space pos of this hex
   public Vector3 WorldPosition()
   {
      float radius = 1.0f;
      float height = radius * 2;
      float width = WIDTH_MULTIPLIER * height;

      float horizontal = width;
      float vertical = height * 0.75f;
      
      var pos = new Vector3(horizontal * (this.Q - this.R % 2 / 2f), 0, -vertical * this.R);
      return pos;
   }

   public void SetResourceYield(List<MapResourceBase> resources)
   {
      AvailableResources = resources;
   }
   
   public void SetWalkableType(bool walkable)
   {
      IsWalkable = walkable;
   }
}
