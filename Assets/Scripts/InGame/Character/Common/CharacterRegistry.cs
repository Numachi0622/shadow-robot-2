using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public static class CharacterRegistry
    {
        private static List<CharacterCore> _players = new();
        private static List<CharacterCore> _enemies = new();
        private static List<CharacterCore> _buildings = new();

        public static void Register(CharacterCore character)
        {
            switch (character)
            {
                case PlayerCore:
                    _players.Add(character);
                    break;
                case EnemyCore:
                    _enemies.Add(character);
                    break;
                // case BuildingCore:
                //     _buildings.Add(character);
                //     break;
            }
        }
        
        public static IReadOnlyList<CharacterCore> GetAllPlayers() => _players;

        public static CharacterCore GetNearestPlayer(Vector3 position)
        {
            return _players.Where(p => p != null)
                .OrderBy(p => (p.transform.position - position).sqrMagnitude)
                .FirstOrDefault();
        }

        public static IReadOnlyList<CharacterCore> GetAllBuildings() => _buildings;
        
        public static CharacterCore GetNearestBuilding(Vector3 position)
        {
            return _buildings.Where(b => b != null)
                .OrderBy(b => (b.transform.position - position).sqrMagnitude)
                .FirstOrDefault();
        }
    }
}