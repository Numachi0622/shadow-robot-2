using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;
using InGame.Character;
using Unity.VisualScripting;
using Utility;

namespace InGame.System
{
    public class CharacterRegistry : ITickable, IDisposable
    {
        private readonly List<CharacterCore> _allCharacters = new();
        private readonly List<CharacterCore> _playerCores = new();
        private readonly Dictionary<AreaId, List<CharacterCore>> _enemyCores = new();
        private readonly List<CharacterCore> _buildingCores = new();
        
        public void Tick()
        {
            foreach (var character in _allCharacters)
            {
                character.OnUpdate();
            }
        }

        public void Dispose()
        {
            _allCharacters.Clear();
            _playerCores.Clear();
            _enemyCores.Clear();
            _buildingCores.Clear();
        }

        public void Register(CharacterCore character)
        {
            _allCharacters.Add(character);
            switch (character)
            {
                case PlayerCore:
                    _playerCores.Add(character);
                    break;
                case BuildingCore:
                    _buildingCores.Add(character);
                    break;
            }
        }

        public void Register(CharacterCore character, AreaId areaId)
        {
            _allCharacters.Add(character);
            if (!_enemyCores.ContainsKey(areaId))
            {
                _enemyCores[areaId] = new List<CharacterCore>();
            }
            _enemyCores[areaId].Add(character);
        }

        public void Remove(CharacterCore character)
        {
            _allCharacters.Remove(character);
            switch (character)
            {
                case PlayerCore:
                    _playerCores.Remove(character);
                    break;
                case BuildingCore:
                    _buildingCores.Remove(character);
                    break;
            }
        }

        public void Remove(CharacterCore character, AreaId areaId)
        {
            _allCharacters.Remove(character);
            _enemyCores.GetValueOrDefault(areaId)?.Remove(character);
        }

        public void RemoveAt(int characterId)
        {
            var character = _playerCores.FirstOrDefault(p => (p as PlayerCore)?.PlayerId.Value == characterId);
            if (character == null) return;
            _playerCores.Remove(character);
            _allCharacters.Remove(character);
            
            (character as PlayerCore)?.Dispose();
        }
        
        public IReadOnlyList<CharacterCore> GetAllPlayers() => _playerCores;

        public CharacterCore GetNearestPlayer(Vector3 position)
        {
            return _playerCores.Where(p => p != null)
                .OrderBy(p => (p.transform.position - position).sqrMagnitude)
                .FirstOrDefault();
        }

        public IReadOnlyList<CharacterCore> GetAllBuildings() => _buildingCores;
        
        public CharacterCore GetNearestBuilding(Vector3 position)
        {
            return _buildingCores.Where(b => b != null)
                .OrderBy(b => (b.transform.position - position).sqrMagnitude)
                .FirstOrDefault();
        }
        
        public IReadOnlyList<CharacterCore> GetEnemiesByArea(AreaId areaId)
        {
            return _enemyCores.GetValueOrDefault(areaId);
        }

        public bool IsEnemyFullByArea(AreaId areaId)
        {
            if (!_enemyCores.TryGetValue(areaId, out var enemies))
            {
                return false;
            }
            
            return enemies.Count >= GameConst.NormalEnemyMaxCountPerArea;
        }
    }
}