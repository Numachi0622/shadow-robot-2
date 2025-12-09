using InGame.Character;
using UnityEngine;

namespace InGame.Message
{
    public readonly struct CreateBuildingMessage
    {
        public readonly BuildingCore BuildingCore;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Transform Parent;
        public CreateBuildingMessage(BuildingCore buildingCore, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            BuildingCore = buildingCore;
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }
    }
}