using TripleTriad.Cards;
using UnityEngine;

namespace TripleTriad
{
    public interface ICardHolder
    {
        CardData Card { get; }
    }

    // カードの属性のタイプ
    public enum ElementType
    {
        Neutral, // 無
        Fire, // 火
        Water, // 水
        Grass, // 草
        Light, // 光
        Darkness // 闇
    }

    // カードのランク
    public enum CardRankType
    {
        OneStar,   // ★☆☆
        TwoStar,   // ★★☆
        ThreeStar  // ★★★
    }

    namespace Cards
    {
        [CreateAssetMenu(fileName = "NewCard", menuName = "Triple Triad/Card")]
        public class CardData : ScriptableObject
        {
            [Header("属性")]
            [SerializeField] ElementType cardElement;
            public ElementType GetCardElement => cardElement;

            [Header("ランク")]
            [SerializeField] CardRankType cardRank;
            public CardRankType GetCardRank => cardRank;

            [Header("カード名")]
            [SerializeField] string cardName;
            public string GetCardName => cardName;

            [Header("カードイメージ")]
            [SerializeField] Sprite cardSprite;
            public Sprite GetCardSprite => cardSprite;

            [Header("パワー")]
            [SerializeField, Range(1, 9)] int topPower;
            [SerializeField, Range(1, 9)] int bottomPower;
            [SerializeField, Range(1, 9)] int leftPower;
            [SerializeField, Range(1, 9)] int rightPower;
            public int GetTopPower => topPower;
            public int GetBottomPower => bottomPower;
            public int GetLeftPower => leftPower;
            public int GetRightPower => rightPower;

            // 合計値
            int totalPower => topPower + bottomPower + leftPower + rightPower;
            public int GetTotalPower => totalPower;

            [Header("フレーバーテキスト")]
            [SerializeField, TextArea] string cardFlavorText;
            public string GetCardFlavorText => cardFlavorText;
        }
    }
}
