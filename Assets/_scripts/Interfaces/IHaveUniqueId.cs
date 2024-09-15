using System;
namespace Game.Interfaces
{
    public interface IHaveUniqueId
    {
        Guid UniqueID { get; set; }
        void GenerateUniqueID();
    }
}