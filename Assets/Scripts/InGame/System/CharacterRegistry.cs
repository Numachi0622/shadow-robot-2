using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;
using InGame.Character;
using Utility;

namespace InGame.System
{
    public class CharacterRegistry : ITickable, IDisposable
    {
        private readonly List<CharacterCore> _allCharacters = new();
        private readonly List<CharacterCore> _playerCores = new();
        private readonly Dictionary<AreaId, List<CharacterCore>> _enemyCores = new();
        private readonly Dictionary<AreaId, List<CharacterCore>> _buildingCores = new();
        
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
            _playerCores.Add(character);
        }

        public void Register(CharacterCore character, AreaId areaId)
        {
            _allCharacters.Add(character);

            switch (character)
            {
                case EnemyCore:
                    if (!_enemyCores.ContainsKey(areaId))
                    {
                        _enemyCores[areaId] = new List<CharacterCore>();
                    }
                    _enemyCores[areaId].Add(character);
                    break;
                
                case BuildingCore:
                    if (!_buildingCores.ContainsKey(areaId))
                    {
                        _buildingCores[areaId] = new List<CharacterCore>();
                    }
                    _buildingCores[areaId].Add(character);
                    break;
            }
        }

        public void Remove(CharacterCore character)
        {
            _allCharacters.Remove(character);
            _playerCores.Remove(character);
        }

        public void Remove(CharacterCore character, AreaId areaId)
        {
            _allCharacters.Remove(character);
            switch (character)
            {
                case EnemyCore:
                    _enemyCores.GetValueOrDefault(areaId)?.Remove(character);
                    break;
                case BuildingCore:
                    _buildingCores.GetValueOrDefault(areaId)?.Remove(character);
                    break;
            }
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
        public IReadOnlyList<CharacterCore> GetAllEnemies() => _enemyCores.Values.SelectMany(list => list).ToList();

        public CharacterCore GetNearestPlayer(Vector3 position)
        {
            return _playerCores.Where(p => p != null)
                .OrderBy(p => (p.transform.position - position).sqrMagnitude)
                .FirstOrDefault();
        }

        public IReadOnlyList<CharacterCore> GetBuildings(AreaId areaId) => _buildingCores.GetValueOrDefault(areaId);
        
        public CharacterCore GetNearestBuilding(AreaId areaId, Vector3 position)
        {
            return _buildingCores
                .GetValueOrDefault(areaId)
                .Where(b => b != null)
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