using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TripleTriad.Cards;
using TripleTriad;
namespace TripleTriad
{
    /// <summary>
    /// カード情報を表示するクラス
    /// </summary>
    public class GameDisplayCard : DisplayCard
    {
        [SerializeField] CardOwnerType cardOwner;

        PlayableCard playableCard;

        [SerializeField] Button detailsButton;

        [SerializeField] GameObject detailsObject;

        [SerializeField] GameCardDetailsDisplay gameCardDetailsDisplay;

        private void Start()
        {
            detailsButton.onClick.AddListener(OnDetailsObject);
        }
        private void FixedUpdate()
        {
            if (playableCard != null)
            {
                detailsButton.interactable = true;
            }
            else
            {
                detailsButton.interactable = false;
            }

            if (playableCard != GameManager.instance.SelectedPlayableCardData &&
               GameManager.instance.SelectedPlayableCardData != null)
            {
                PlayingGameSelectCardDisplay();
            }
        }

        public void PlayingGameSelectCardDisplay()
        {
            if (cardOwner == GameManager.instance.SelectedPlayableCardData.CardCurrentOwner)
            {
                SetDisplayCard(GameManager.instance.SelectedPlayableCardData);
                playableCard = GameManager.instance.SelectedPlayableCardData;
            }
        }

        void OnDetailsObject()
        {
            detailsObject.SetActive(true);
            gameCardDetailsDisplay.SetDisplayCard(playableCard);
            Time.timeScale = 0;
        }
    }
}

public abstract class DisplayCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI topText;
    [SerializeField] TextMeshProUGUI bottomText;
    [SerializeField] TextMeshProUGUI leftText;
    [SerializeField] TextMeshProUGUI rightText;
    [SerializeField] Image cardImage;
    [SerializeField] Image elementImage;

    public virtual void SetDisplayCard(PlayableCard card)
    {
        nameText.text = card.Card.GetCardName;
        //--------------------
        topText.text = card.TopPower.ToString();
        topText.gameObject.SetActive(true);
        //--------------------
        bottomText.text = card.BottomPower.ToString();
        bottomText.gameObject.SetActive(true);
        //--------------------
        leftText.text = card.LeftPower.ToString();
        leftText.gameObject.SetActive(true);
        //--------------------
        rightText.text = card.RightPower.ToString();
        rightText.gameObject.SetActive(true);
        //--------------------
        cardImage.sprite = card.Card.GetCardSprite;
        cardImage.gameObject.SetActive(true);
        //--------------------
        elementImage.sprite = GameManager.instance.elementSpritePair[card.Card.GetCardElement];
        elementImage.gameObject.SetActive(true);
    }
}