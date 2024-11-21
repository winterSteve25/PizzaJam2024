using System;
using Combat;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RoundUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            var p = text.rectTransform.pivot;
            p.y = -1;
            text.rectTransform.pivot = p;
        }

        private void OnEnable()
        {
            CombatManager.Current.OnRoundChanged += UpdateUI;
        }

        private void OnDisable()
        {
            CombatManager.Current.OnRoundChanged -= UpdateUI;
        }

        private void UpdateUI(int round)
        {
            text.text = $"Round: {round}";
            text.rectTransform.DOPivotY(1, 0.2f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    text.rectTransform.DOPivotY(-1, 0.2f)
                        .SetEase(Ease.OutCubic)
                        .SetDelay(1f);
                });
        }
    }
}