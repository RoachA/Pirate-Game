public interface IMapResource
{
   public float Value { get; set; }
   public MapResourceType ResourceType { set; get; }
}

public enum MapResourceType
{
   Gold,
   Food,
}

