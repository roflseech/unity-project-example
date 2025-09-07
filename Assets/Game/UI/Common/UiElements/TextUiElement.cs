using Cysharp.Text;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace Game.UI.Common.UiElements
{
    public class TextUiElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetString(string text)
        {
            _text.text = text;
        }
        
        public void SetString(in Utf16ValueStringBuilder sb)
        {
            _text.SetText(sb);
        }

        public void SetInt(int value)
        {
            using var sb = ZString.CreateStringBuilder();
            sb.Append(value);
            _text.SetText(sb);
        }

        public void Clear()
        {
            _text.text = string.Empty;
        }
    }
}