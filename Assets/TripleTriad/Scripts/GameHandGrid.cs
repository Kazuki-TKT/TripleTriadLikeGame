using System.Collections;
using System.Collections.Generic;
using TripleTriad.Area;
using TripleTriad.Bord;
using TripleTriad.Cards;
using UnityEngine;

namespace TripleTriad
{
    public class GameHandGrid : MonoBehaviour, IGridInterface<GameHandCell>
    {
        public GameHandCell[,] Grid { get; private set; }

        public int Rows { get; private set; } = 2;

        public int Columns { get; private set; } = 3;

        private void Awake()
        {
            Grid = new GameHandCell[Rows, Columns];
        }

        void Start()
        {

        }

        public IEnumerator SetHandCoroutin(List<CardData> cardDatas, CardOwnerType cardOwnerType)
        {
            // セルの情報が全て取得できるまで繰り返す
            while (true)
            {
                if (CheckCellsCondition())
                {
                    break;
                }
                yield return null;
            }
            // カードをセットする
            SetHand(GetAllHandCells(), cardDatas, cardOwnerType);
        }

        void SetHand(GameHandCell[] cells, List<CardData> cardDatas, CardOwnerType cardOwnerType)
        {
            if (GameManager.instance.CardPrefab != null)
            {
                var cardObject = GameManager.instance.CardPrefab;
                for (int i = 0; i < cardDatas.Count; i++)
                {
                    // カードの作成
                    var card = Instantiate(
                        cardObject,
                        cells[i].gameObject.transform.position,
                        cells[i].gameObject.transform.rotation,
                        this.transform);
                    // Playableクラスの取得
                    PlayableCard playableCard = card.GetComponent<PlayableCard>();
                    // AreaCardの設定
                    cells[i].AreaCard = playableCard;
                    // カードデータを設定
                    playableCard.Card = cardDatas[i];
                    // カードの所持者の設定
                    playableCard.CardCurrentOwner = cardOwnerType;
                    // カードのデータを反映
                    playableCard.InitializeCardSettings();
                    // PlayableCardのGameHandCellを代入
                    playableCard.HandCell = cells[i];
                    // オーバーラッピング用のトランスフォームを設定
                    cells[i].handCardRectTransform = playableCard.RectOverlapTransform;
                    // 生成したオブジェクトの名前の変更
                    card.gameObject.name = playableCard.Card.GetCardName;
                }
            }
        }

        // 指定したセルの情報を入手する
        public GameHandCell GetCell(int row, int col)
        {
            if (row >= 0 && row < Grid.GetLength(0) && col >= 0 && col < Grid.GetLength(1))
            {
                return Grid[row, col];
            }
            return null;
        }

        // 全ての手札のセルの情報を入手する
        GameHandCell[] GetAllHandCells()
        {
            GameHandCell[] cells = {
            GetCell(0, 0), GetCell(0, 1), GetCell(0, 2), GetCell(1, 0), GetCell(1, 1)
            };
            return cells;
        }

        public List<PlayableCard> GetCurrentHandCards()
        {
            List<PlayableCard> cards = new List<PlayableCard>();
            foreach (GameHandCell cell in GetAllHandCells())
            {
                if (cell.AreaCard != null) cards.Add(cell.AreaCard);
            }
            return cards;
        }

        // 全てのセルがＮＵＬＬじゃないかチェックする
        bool CheckCellsCondition()
        {
            foreach (var cell in GetAllHandCells())
            {
                if (cell == null)
                {
                    return false; // いずれかのセルの値が条件を満たしていない場合
                }
            }
            return true; // 全てのセルが条件を満たしている場合
        }
    }
}
