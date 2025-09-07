using Game.AssetManagement;
using Game.UI.Common.Inject;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI.Common.UiElements
{
    public class ImageUiElement : InjectableUiElement
    {
        [SerializeField] private Image _image;
        
        private readonly CompositeDisposable _disposables = new();
        
        private ISpriteProvider _spriteProvider;
        
        
        [Inject]
        public void Construct(ISpriteProvider spriteProvider)
        {
            _spriteProvider = spriteProvider;
        }
        
        protected virtual void OnDestroy()
        {
            _disposables?.Dispose();
        }
        
        public void SetImage(string path)
        {
            _disposables.Clear();
            
            _image.sprite = null;
            _image.enabled = false;
            
            if (string.IsNullOrEmpty(path))
            {
                _image.sprite = null;
                _image.enabled = false;
            }
            else
            {
                _image.enabled = true;
                _spriteProvider.GetSpriteAsObservable(path)
                    .Subscribe(sprite =>
                    {
                        _image.sprite = sprite;
                    })
                    .AddTo(_disposables);
            }
        }
    }
}