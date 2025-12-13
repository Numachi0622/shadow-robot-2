using InGame.Character;
using InGame.System;
using UnityEngine;

namespace InGame.Message
{
    public readonly struct CreateBuildingMessage
    {
        public readonly BuildingCore BuildingCore;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Transform Parent;
        public readonly AreaId AreaId;
        public CreateBuildingMessage(BuildingCore buildingCore, AreaId areaId, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            BuildingCore = buildingCore;
            AreaId = areaId;
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }
    }
}