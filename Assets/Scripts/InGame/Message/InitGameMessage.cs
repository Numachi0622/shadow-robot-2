using InGame.System;

namespace InGame.Message
{
    public readonly struct InitGameMessage : IStateParameter
    {
        public readonly int PlayerCount;
        public InitGameMessage(int playerCount)
        {
            PlayerCount = playerCount;
        }
    }
}