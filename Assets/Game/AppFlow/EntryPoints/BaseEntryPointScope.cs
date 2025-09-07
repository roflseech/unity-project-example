using UnityEngine;
using VContainer.Unity;

namespace Game.AppFlow.EntryPoints
{
    public class BaseEntryPointScope : LifetimeScope
    {
        private async void Start()
        {
            var root = VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance();

            if (root != null && root is RootInstaller rootInstaller)
            {
                await rootInstaller.InitializeAsync();
            }
            else
            {
                Debug.LogError("Root is not installed");
                return;
            }
            
            Build();
        }
    }
}