using System.Collections;
using System.Collections.Generic;
using TMPro;
using TripleTriad.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace TripleTriad
{
    /// <summary>
    /// ゲームカードの詳細情報を表示するクラス
    /// </summary>
    public class GameCardDetailsDisplay : DisplayCard
    {

        [SerializeField] TextMeshProUGUI flavorText;

        [SerializeField] Button closeButton;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseDetailsObject);
            gameObject.SetActive(false);
        }

        public override void SetDisplayCard(PlayableCard card)
        {
            base.SetDisplayCard(card);
            flavorText.text = card.Card.GetCardFlavorText;
        }

        void CloseDetailsObject()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
