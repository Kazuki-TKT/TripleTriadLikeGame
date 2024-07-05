using System.Collections;
using System.Collections.Generic;
using TMPro;
using TripleTriad.Area;
using UnityEngine;
using UnityEngine.UI;

namespace TripleTriad.Cards
{
    // カードの所持者
    public enum CardOwnerType
    {
        None, //　誰でもない 
        Player, // プレイヤー
        CPU // CPU
    }

    /// <summary>
    /// カードのベースとなるクラス。カードの情報をまとめる
    /// </summary>
    public class CardBase : CardElementTypeMatchup, ICardHolder
    {
        [SerializeField, Tooltip("非再生時でもカードを生成することができます")]
        bool debugCreate;

        //--------------------
        [Header("現在のカードの所持者"), SerializeField] CardOwnerType currentCardOwner; // 現在のカードのオーナー
        public CardOwnerType CardCurrentOwner { get => currentCardOwner; set => currentCardOwner = value; }
        //--------------------

        //--------------------
        // 属性を日本語で扱う時に使用
        public string GetAttributeJapaneseText { get; private set; }
        const string Neutral_Attribute = "無";
        const string Fire_Attribute = "火";
        const string Water_Attribute = "水";
        const string Grass_Attribute = "草";
        const string Light_Attribute = "光";
        const string Darkness_Attribute = "闇";
        //--------------------

        [Header("カードのデータ")]
        [SerializeField] protected CardData cardData; // このカードのデータ（ScriptableObject）
        public CardData Card { get => cardData; set => cardData = value; }
        //public CardData Card { get => cardData; set => cardData = value; }


        [Header("カードのテキスト情報を表示するUI")]
        [SerializeField] Image cardImage;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI flavorText;
        [SerializeField] protected TextMeshProUGUI topPowerText;
        [SerializeField] protected TextMeshProUGUI bottomPowerText;
        [SerializeField] protected TextMeshProUGUI leftPowerText;
        [SerializeField] protected TextMeshProUGUI rightPowerText;

        [Header("属性のアイコン")]
        [SerializeField] Image elementIcon;

        [Header("ランクオブジェクトの配列")]
        [SerializeField] GameObject[] rankObjects;

        //--------------------
        [Header("カードのFrameMateriak")]
        [SerializeField] Material defaultCardFrameMaterial;
        [SerializeField] Material blueCardFrameMaterial;
        [SerializeField] Material redCardFrameMaterial;
        [SerializeField] GameObject selectedOutlineImage;
        public GameObject SelectedOutlineImage => selectedOutlineImage;
        //--------------------

        // 特定のパワーとそのパワーを表示しているテキストのディクショナリー
        protected Dictionary<DirectionType, (int power, TextMeshProUGUI powerText)> directionData;
        public Dictionary<DirectionType, (int power, TextMeshProUGUI powerText)> DirectionData => directionData;

        private void Update()
        {
            if (debugCreate)
            {
                Debug.Log("カードの作成");
                DisplayCard();
                debugCreate = false;
            }
        }

        // カードの上表をUIに反映させる
        public void DisplayCard()
        {
            if (cardData != null)
            {
                selectedOutlineImage.SetActive(false);
                SetElementIconAndJapaneseText();
                SetRankIcon();
                ChangeCardFrame();
                if (cardImage != null) cardImage.sprite = cardData.GetCardSprite;
                if (nameText != null) nameText.text = cardData.GetCardName;
                if (flavorText != null) flavorText.text = cardData.GetCardFlavorText;
                SetDefaultPower();
                directionData = new Dictionary<DirectionType, (int power, TextMeshProUGUI powerText)>
                 {
                  { DirectionType.Top, (cardData.GetTopPower, topPowerText) },
                  { DirectionType.Bottom, (cardData.GetBottomPower, bottomPowerText) },
                  { DirectionType.Left, (cardData.GetLeftPower, leftPowerText) },
                  { DirectionType.Right, (cardData.GetRightPower, rightPowerText) }
                 };
            }
            else
            {
                Debug.LogError("カードのデータがありません");
            }
        }

        // カードの属性のアイコンをセットし日本語の属性も変更
        void SetElementIconAndJapaneseText()
        {
            elementIcon.sprite = GameManager.instance.elementSpritePair[cardData.GetCardElement];
            // 属性に沿ったシンボルの表示
            switch (cardData.GetCardElement)
            {
                case ElementType.Neutral:
                    GetAttributeJapaneseText = Neutral_Attribute;
                    break;
                case ElementType.Fire:
                    GetAttributeJapaneseText = Fire_Attribute;
                    break;
                case ElementType.Water:
                    GetAttributeJapaneseText = Water_Attribute;
                    break;

                case ElementType.Grass:
                    GetAttributeJapaneseText = Water_Attribute;
                    break;
                case ElementType.Light:
                    GetAttributeJapaneseText = Light_Attribute;
                    break;
                case ElementType.Darkness:
                    GetAttributeJapaneseText = Darkness_Attribute;
                    break;
            }
        }

        // ランクのオブジェクトをセットする
        void SetRankIcon()
        {
            if (rankObjects != null && rankObjects.Length > 0)
            {
                //非表示
                foreach (GameObject obj in rankObjects)
                {
                    obj.SetActive(false);
                }

                // 属性に沿ったシンボルの表示
                switch (cardData.GetCardRank)
                {
                    case CardRankType.OneStar:
                        rankObjects[0].SetActive(true);
                        break;
                    case CardRankType.TwoStar:
                        rankObjects[1].SetActive(true);
                        break;
                    case CardRankType.ThreeStar:
                        rankObjects[2].SetActive(true);
                        break;
                }
            }
            else
            {
                Debug.LogWarning("ランクオブジェクトをセットしてください");
            }
        }

        // マテリアルを変更する
        public void ChangeCardFrame()
        {
            if (cardImage != null)
            {
                switch (currentCardOwner)
                {
                    case CardOwnerType.Player:
                        cardImage.material = redCardFrameMaterial;
                        break;
                    case CardOwnerType.CPU:
                        cardImage.material = blueCardFrameMaterial;
                        break;
                    case CardOwnerType.None:
                        cardImage.material = defaultCardFrameMaterial;
                        break;
                }
            }
        }

        public void SetDefaultPower()
        {
            if (topPowerText != null) topPowerText.text = cardData.GetTopPower.ToString();
            if (bottomPowerText != null) bottomPowerText.text = cardData.GetBottomPower.ToString();
            if (leftPowerText != null) leftPowerText.text = cardData.GetLeftPower.ToString();
            if (rightPowerText != null) rightPowerText.text = cardData.GetRightPower.ToString();
        }
    }
}