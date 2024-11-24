using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Combat.DmgNumber
{
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Color healColor;
        [SerializeField] private Color dmgColor;

        public void Init(string txt, Vector3 position, Action release, bool heal, bool crit)
        {
            text.color = heal ? healColor : dmgColor;
            canvasGroup.alpha = 1;
            text.text = crit ? $"{txt}!!" : txt;
            transform.position = position;
            canvasGroup.DOFade(0, 2)
                .OnComplete(() => release());
            transform.DOMoveY(position.y + 1, 2.1f)
                .SetEase(Ease.OutCubic);
        }
    }
}