using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class CharacterRegistry : Singleton<CharacterRegistry>
    {
        private List<CharacterCore> _players = new();
        private List<CharacterCore> _enemies = new();
        private List<CharacterCore> _buildings = new();
    }
}