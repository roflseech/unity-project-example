using UnityEngine;

namespace Game.UI.Common.Inject
{
    public class InjectableUiElement : MonoBehaviour
    {
        protected virtual void Awake()
        {
            UiInjector.Injector.Inject(this);
        }
    }
}