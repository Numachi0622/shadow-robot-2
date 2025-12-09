using UnityEngine;

namespace InGame.Message
{
    public readonly struct GameStartPlayerInitMessage
    {
        public readonly Vector3 GameStartPosition;
        public readonly int TotalPlayerCount;
        public GameStartPlayerInitMessage(Vector3 gameStartPosition, int totalPlayerCount)
        {
            GameStartPosition = gameStartPosition;
            TotalPlayerCount = totalPlayerCount;
        }
    }
}