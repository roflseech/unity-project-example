using Cysharp.Threading.Tasks;
using Game.AssetManagement;
using Game.Gameplay.Entity;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.ContentProvider
{
    public interface IEntityFactory
    {
        UniTask Initialize();
        IPlayer CreatePlayer();
    }
    
    public class EntityFactory : IEntityFactory
    {
        private IAssetProvider _assetProvider;
        private EntityConfig _entityConfig;
        private IObjectResolver _objectResolver;
        
        public EntityFactory(IAssetProvider assetProvider, EntityConfig entityConfig, IObjectResolver objectResolver)
        {
            _assetProvider = assetProvider;
            _entityConfig = entityConfig;
            _objectResolver = objectResolver;
        }

        public async UniTask Initialize()
        {
            await _assetProvider.PreloadAsset<GameObject>(_entityConfig.PlayerAsset);
        }

        public IPlayer CreatePlayer()
        {
            var prefab = _assetProvider.GetAsset<GameObject>(_entityConfig.PlayerAsset);
            var inst = GameObject.Instantiate(prefab);
            _objectResolver.InjectGameObject(inst.gameObject);
            
            return inst.GetComponent<IPlayer>();
        }
    }
}