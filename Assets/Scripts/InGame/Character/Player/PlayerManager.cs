using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace InGame.Character
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerCore _playerPrefab;
        [SerializeField] private int _playerCount = 3;

        private List<PlayerCore> _players = new List<PlayerCore>();
        private MotionReceiver _motionReceiver;

        public void Initialize()
        {
        }

        private void Generate()
        {
            var player = Instantiate(_playerPrefab).GetComponent<PlayerCore>();
        }

        public void OnUpdate()
        {
            _motionReceiver.UpdateMotion();
        }
    }
}