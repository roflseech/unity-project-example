using Cysharp.Threading.Tasks;
using Game.Common.VContainer;
using Game.Gameplay.CameraManagement;
using Game.Gameplay.ContentProvider;
using Game.Gameplay.Controls;
using Game.Gameplay.Models.Player;
using Game.Models.InventoryManagement;
using Game.UI.GameModels.Windows;
using Game.UI.Provider;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.AppFlow.EntryPoints
{
    public class GameplayEntryPointScope : BaseEntryPointScope
    {
        [SerializeField] private Transform _playerSpawnPosition;
        [SerializeField] private CameraProvider _cameraProvider;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.BindSingleton<ControlsProvider>();
            builder.BindSingleton<EntityFactory>();
            builder.BindConfigSO<EntityConfig>("Configs/EntityConfig");
            builder.BindSingleton<PlayerControlsManager>();
            
            builder.BindComponent(_cameraProvider);

            builder.RegisterEntryPoint<EntryPoint>();
        }

        private class EntryPoint : BaseEntryPoint
        {
            private readonly IUiProvider _uiProvider;
            private readonly IEntityFactory _entityFactory;
            private readonly IPlayerControlsManager _playerControls;
            private readonly ControlsProvider _controlsProvider;
            private readonly ICameraProvider _cameraProvider;
            private readonly IPlayerData _playerData;
            private readonly IBaseGameplayWindowModel _baseGameplayWindowModel;
            
            public EntryPoint(ICoreInitializer coreInitializer, IUiProvider uiProvider,
                IEntityFactory entityFactory, IPlayerControlsManager playerControls,
                ControlsProvider controlsProvider, ICameraProvider cameraProvider,
                IPlayerData playerData, IBaseGameplayWindowModel baseGameplayWindowModel) : base(coreInitializer)
            {
                _entityFactory = entityFactory;
                _playerControls = playerControls;
                _controlsProvider = controlsProvider;
                _cameraProvider = cameraProvider;
                _uiProvider = uiProvider;
                _playerData = playerData;
                _baseGameplayWindowModel = baseGameplayWindowModel;
            }

            protected override async UniTask InitializeAsync()
            {
                _controlsProvider.Init();
                await _entityFactory.Initialize();
            
                var player = _entityFactory.CreatePlayer();

                player.GameObject.transform.position = Vector3.zero;
                _playerControls.AttachPlayer(player);
                _cameraProvider.SetTopDownFollowTarget(player.GameObject);
            
                _playerData.Inventory.Inventory.AddItem(new ItemEntry(new BaseItem(1), 3));
                _playerData.Inventory.Inventory.AddItem(new ItemEntry(new BaseItem(2), 15));
                _baseGameplayWindowModel.AttachInventory(_playerData.Inventory);
            
                _uiProvider.OpenSingletonWindow<IBaseGameplayWindowModel>();
            }
        }
    }
}