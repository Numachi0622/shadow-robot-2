using System;
using InGame.System;

namespace InGame.Message
{
    /// <summary>
    /// State変更リクエスト
    /// </summary>
    public struct StateChangeMessage
    {
        public readonly GameStateType StateType;
        public readonly IStateParameter Parameter;

        public StateChangeMessage(GameStateType stateType, IStateParameter parameter = null)
        {
            StateType = stateType;
            Parameter = parameter;
        }
    }
    
    public enum GameStateType
    {
        Title = 0,
        NormalBattle = 1,
        BossBattle = 2,
        Result = 3,
        GameOver = 4
    }
}