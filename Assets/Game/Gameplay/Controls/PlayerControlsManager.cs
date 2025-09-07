using System.Collections.Generic;
using Game.Gameplay.CameraManagement;
using Game.Gameplay.Entity;
using Game.Gameplay.Models.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Gameplay.Controls
{
    public interface IPlayerControlsManager
    {
        void AttachPlayer(IPlayer player);
    }
    
    public class PlayerControlsManager : IPlayerControlsManager, ITickable
    {
        private readonly ControlsProvider _controlsProvider;
        private readonly ICameraProvider _cameraProvider;
        private readonly IPlayerData _playerData;
        
        private IPlayer _player;

        private List<InputAction> _itemSlotKeys;
        
        public PlayerControlsManager(ControlsProvider controlsProvider, ICameraProvider cameraProvider)
        {
            _controlsProvider = controlsProvider;
            _cameraProvider = cameraProvider;
        }

        public void AttachPlayer(IPlayer player)
        {
            _player = player;
        }

        public void Tick()
        {
            if (_player == null || _player.GameObject == null) return;
            var move = _controlsProvider.Controls.Player.Move.ReadValue<Vector2>();
            _player.Move(move);
            
            var mouseScreenPoint = _controlsProvider.Controls.Player.MouseLook.ReadValue<Vector2>();
            var camera = _cameraProvider.Camera;
            if (camera != null)
            {
                var ray = camera.ScreenPointToRay(mouseScreenPoint);
                if (Mathf.Abs(ray.direction.y) > 0.001f)
                {
                    var distance = -ray.origin.y / ray.direction.y;
                    var groundPoint = ray.origin + ray.direction * distance;
                    _player.LookAt(groundPoint);
                }
            }

            var clickedSlot = InventorySlotClicked();

            if (clickedSlot >= 0)
            {
                _playerData.Inventory.Selection.SelectSlot(clickedSlot);
            }
        }

        private int InventorySlotClicked()
        {
            if (_itemSlotKeys == null)
            {
                _itemSlotKeys = new();
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item1);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item2);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item3);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item4);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item5);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item6);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item7);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item8);
                _itemSlotKeys.Add(_controlsProvider.Controls.Player.Item9);
            }

            for (int i = 0; i < _playerData.Inventory.Inventory.SlotsCount; i++)
            {
                if (_itemSlotKeys.Count <= i)
                {
                    Debug.LogError("Player inventory has more slots than buttons");
                    return -1;
                }
                
                if (_itemSlotKeys[i].IsPressed()) return i;
            }

            return -1;
        }
    }
}