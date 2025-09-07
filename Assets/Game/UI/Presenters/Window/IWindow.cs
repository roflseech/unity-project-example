using Game.UI.Models.Window;
using UnityEngine;

namespace Game.UI.Presenters.Window
{
    public interface IWindow : IBindable
    {
        Transform Transform { get; }
        void OnOpen();
        void OnClose();
    }

    public interface IWindow<in T> : IWindow, IBindable<T> where T : IWindowModel
    {
    }
}