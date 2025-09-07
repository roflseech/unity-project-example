using Game.Localization;
using Game.UI.Common.UiElements;
using Game.UI.GameModels.Widgets;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;
using VContainer;
using Cysharp.Text;
using System.Linq;

namespace Game.UI.GamePresenters.Widgets
{
    public class TextWidget : BaseWidget<ITextWidgetModel>
    {
        [SerializeField] private TextUiElement _textUiElement;
        
        private ILocalizationProvider _localizationProvider;
        
        [Inject]
        public void Construct(ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider;
        }
        
        protected override void SetBindings(ITextWidgetModel model, CompositeDisposable bindings)
        {
            if (model.IsLocalized)
            {
                var localizedText = _localizationProvider.GetLocalized(model.Text);
                
                if (string.IsNullOrEmpty(localizedText))
                {
                    SetErrorText(model.Text, model.Arguments);
                    return;
                }
                
                if (model.Arguments != null && model.Arguments.Length > 0)
                {
                    using var sb = ZString.CreateStringBuilder();
                    sb.AppendFormat(localizedText, model.Arguments);
                    _textUiElement.SetString(in sb);
                }
                else
                {
                    _textUiElement.SetString(localizedText);
                }
                
                return;
            }

            _textUiElement.SetString(model.Text);
        }
        
        private void SetErrorText(string text, string[] args)
        {
            using var sb = ZString.CreateStringBuilder();
            sb.Append("(!)");
            sb.Append(text);
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    sb.Append('[');
                    sb.Append(args[i]);
                    sb.Append(']');
                }
            }
            _textUiElement.SetString(in sb);
        }
    }
}