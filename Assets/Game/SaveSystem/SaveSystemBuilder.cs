using System;
using System.Collections.Generic;
using Game.SaveSystem.Storage;
using VContainer;

namespace Game.SaveSystem
{
    public class SaveSystemBuilder
    {
        private readonly IContainerBuilder _containerBuilder;

        private readonly List<Action<SaveDataManager>> _registerActions = new();
        
        public SaveSystemBuilder(IContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Register<T>(string key, T defaultValue = default) where T : struct
        {
            _containerBuilder.Register<SaveData<T>>(Lifetime.Singleton)
                .As<IReadOnlySaveData<T>>()
                .As<ISaveData<T>>();
            
            _registerActions.Add(saveDataManager =>
            {
                saveDataManager.Register<T>(key, defaultValue);
            });
        }

        public void Build()
        {
            _containerBuilder.Register(resolver =>
                {
                    var runtimeStorage = new RuntimeDataStorage();
                    var persistantStorage = new DiskPersistantDataStorage();
                    var saveDataManager = new SaveDataManager(runtimeStorage, persistantStorage, resolver);

                    foreach (var action in _registerActions)
                    {
                        action.Invoke(saveDataManager);
                    }
                    
                    return saveDataManager;
                }, Lifetime.Singleton).As<ISaveDataManager>();
        }
    }
}