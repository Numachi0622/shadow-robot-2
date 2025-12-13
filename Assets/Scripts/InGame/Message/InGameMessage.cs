using InGame.Character;
using InGame.System;

namespace InGame.Message
{
    public readonly struct BuildingDestroyedMessage
    {
        public readonly AreaId AreaId;
        public readonly BuildingCore Building;

        public BuildingDestroyedMessage(AreaId areaId, BuildingCore building)
        {
            AreaId = areaId;
            Building = building;
        }
    }
}