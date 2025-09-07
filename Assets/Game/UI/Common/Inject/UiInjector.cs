using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Common.Inject
{
    internal class UiInjector : MonoBehaviour
    {
        [SerializeField] private LifetimeScope _scope;

        private static UiInjector _instance;
        private static IObjectResolver _injector;
        
        internal static IObjectResolver Injector
        {
            get
            {
                if (_injector == null) _injector = _instance._scope.Container;
                return _injector;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Injector already exists");
            }
            
            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
            _injector = null;
        }
    }
}