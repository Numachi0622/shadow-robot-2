using System.Collections.Generic;
using SynMotion;
using UnityEngine;
using UniRx;

namespace InGame.Character
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private PlayerCore _playerPrefab;
        [SerializeField] private int _playerCount = 3;

        private List<PlayerCore> _players = new List<PlayerCore>();
        private MotionReceiver _motionReceiver;
        private SynMotionSystem _synMotion;

        public void Initialize()
        {
            _synMotion = new SynMotionSystem();
            _motionReceiver = new MotionReceiver(_deviceSettings, _synMotion);
            
            Bind();
        }

        private void Bind()
        {
            for (var i = 0; i < _motionReceiver.ConnectedFlags.Length; i++)
            {
                var playerId = i;
                _motionReceiver.ConnectedFlags[playerId]
                    .Subscribe(isConnected => OnConnected(isConnected, playerId, _synMotion))
                    .AddTo(this);
            }
        }

        private void OnConnected(bool isConnected, int playerId, SynMotionSystem synMotion)
        {
            if (isConnected)
            {
                Generate(playerId, synMotion);
            }
            else
            {
                var player = _players.Find(p => p.PlayerId == playerId);
                if (player == null) return;
                
                _players.Remove(player);
                Destroy(player.gameObject);
            }
        }

        private void Generate(int playerId, SynMotionSystem synMotion)
        {
            var player = Instantiate(_playerPrefab).GetComponent<PlayerCore>();
            player.transform.position = new Vector3(-1 + playerId * 2, 0, 0);
            if (player == null) return;
            player.Initialize(playerId, synMotion);
            _players.Add(player);
        }

        public void OnUpdate()
        {
            _motionReceiver.UpdateMotion();

            foreach (var player in _players)
            {
                player.OnUpdate();
            }
        }
    }
}