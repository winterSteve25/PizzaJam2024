using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ActionButton : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, ISubmitHandler
    {
        public event Action OnClick;
        
        [SerializeField] private RectTransform arrow;
        [SerializeField] private TMP_Text text;
        
        private RectTransform _rect;
        private Image _arrowImg;
        private bool _originalSize;

        public void Init(string text, RectTransform arrow)
        {
            this.text.text = text;
            this.arrow = arrow;
        }

        private void Start()
        {
            _rect = (RectTransform)transform;
            _arrowImg = arrow.GetComponent<Image>();
        }

        private void OnDestroy()
        {
            OnClick = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Submit();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ResetCursor();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _originalSize = false;
            arrow.DOScale(0.013f, 0.1f)
                .SetEase(Ease.OutCubic);
            _arrowImg.DOColor(Color.red, 0.1f);
        }

        public void OnSelect(BaseEventData eventData)
        {
            arrow.DOMoveY(_rect.position.y, 0.1f).SetEase(Ease.OutCubic);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Submit();
        }

        private void Submit()
        {
            OnClick?.Invoke();
            ResetCursor();
        }
        
        private void ResetCursor()
        {
            if (_originalSize) return;
            arrow.DOScale(0.01f, 0.1f)
                .SetEase(Ease.OutCubic);
            _arrowImg.DOColor(Color.white, 0.1f);
        }
    }
}