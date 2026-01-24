using InGame.System;

namespace InGame.Message
{
    public readonly struct InitGameMessage : IStateParameter
    {
        public readonly int PlayerCount;
        public readonly int BuildingCoutPerArea;
        public InitGameMessage(int playerCount, int buildingCoutPerArea)
        {
            PlayerCount = playerCount;
            BuildingCoutPerArea = buildingCoutPerArea;
        }
    }
}