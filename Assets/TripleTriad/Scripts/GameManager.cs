using System.Collections;
using System.Collections.Generic;
using TripleTriad.Area;
using TripleTriad.Cards;
using UnityEngine;

namespace TripleTriad
{
    public enum DirectionType
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public const string Card_Tag = "Card";

        public const string Grid_Tag = "Grid";

        // 現在選択中のプレイアブルカード
        [SerializeField] private PlayableCard selectedPlayableCardData;
        public PlayableCard SelectedPlayableCardData { get => selectedPlayableCardData; set => selectedPlayableCardData = value; }

        // プレイヤー１のデッキ手札
        [SerializeField] private List<CardData> playerHand;
        public List<CardData> PlayerHand { get => playerHand; set => playerHand = value; }

        // CPUのデッキ手札
        [SerializeField] private List<CardData> cpuHand;
        public List<CardData> CpuHand { get => cpuHand; set => cpuHand = value; }

        // カード生成用のカードプレファブ
        [SerializeField] GameObject cardPrefab;
        public GameObject CardPrefab => cardPrefab;

        // 全てのカードデータ
        public CardPool cardPool;

        // 属性のシンボルデータ
        [SerializeField]
        ElementSymbolData elementSymbolData;

        // エレメントの属性の列挙型と対応するスプライト
        public Dictionary<ElementType, Sprite> elementSpritePair;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            elementSpritePair = new Dictionary<ElementType, Sprite>()
            {
                {ElementType.Fire , elementSymbolData.FireSprite},
                {ElementType.Water , elementSymbolData.WaterSprite},
                {ElementType.Grass , elementSymbolData.GrassSprite},
                {ElementType.Darkness,elementSymbolData.DarknessSprite },
                {ElementType.Light,elementSymbolData.LightSprite},
                {ElementType.Neutral , elementSymbolData.NeutralSprite},
            };
        }
    }
}
