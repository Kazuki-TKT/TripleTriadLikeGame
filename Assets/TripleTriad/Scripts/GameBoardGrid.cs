using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleTriad.Cards;
using TripleTriad.Area;

namespace TripleTriad.Bord
{
    public interface IGridInterface<T> where T : class
    {
        public T[,] Grid { get; }

        public int Rows { get; }

        public int Columns { get; }

        T GetCell(int row, int col);
    }

    public class GameBoardGrid : MonoBehaviour, IGridInterface<GameBoardCell>
    {
        public static GameBoardGrid instance;

        public GameBoardCell[,] Grid { get; private set; } // 3x3のグリッド
        public int Rows { get; private set; } = 3; // 行の数
        public int Columns { get; private set; } = 3; // 列の数

        public bool debug = false;

        internal List<PlayableCard> currentCardsData;

        private void Awake()
        {
            Grid = new GameBoardCell[Rows, Columns];
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (debug)
            {
                debug = false;
                DisplayBoard();
            }
        }

        // ゲームボードの状態を表示するメソッド
        public void DisplayBoard()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Grid[row, col].AreaCard != null)
                    {
                        Debug.Log($"Card at [{row},{col}]: {Grid[row, col].AreaCard.Card.GetCardName}");
                    }
                }
            }
        }

        // 現在ボードに配置されているカードを取得
        public void GetCurrentBoardCard()
        {
            currentCardsData = new List<PlayableCard>();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Grid[row, col].AreaCard != null)
                    {
                        currentCardsData.Add(Grid[row, col].AreaCard);
                    }
                }
            }
        }

        // カードから対応する方向にあるカードの配置セルとパワーを取得
        public (GameBoardCell, CardPowerPair) CheckDirectionCardData(DirectionType directionType, int row, int col)
        {
            PlayableCard card = null;
            GameBoardCell bordCell = null;
            CardPowerPair pairList = null;
            int power = 0;

            switch (directionType)
            {
                case DirectionType.Top:
                    bordCell = GetCell(row - 1, col);
                    if (bordCell != null && bordCell.AreaCard != null)
                    {
                        power = bordCell.AreaCard.Card.GetBottomPower;
                        card = bordCell.AreaCard;
                        pairList = (new(card, power));
                    }
                    break;
                case DirectionType.Bottom:
                    bordCell = GetCell(row + 1, col);
                    if (bordCell != null && bordCell.AreaCard != null)
                    {
                        power = bordCell.AreaCard.Card.GetTopPower;
                        card = bordCell.AreaCard;
                        pairList = (new(card, power));
                    }
                    break;
                case DirectionType.Left:
                    bordCell = GetCell(row, col - 1);
                    if (bordCell != null && bordCell.AreaCard != null)
                    {
                        power = bordCell.AreaCard.Card.GetRightPower;
                        card = bordCell.AreaCard;
                        pairList = (new(card, power));
                    }
                    break;
                case DirectionType.Right:
                    bordCell = GetCell(row, col + 1);
                    if (bordCell != null && bordCell.AreaCard != null)
                    {
                        power = bordCell.AreaCard.Card.GetLeftPower;
                        card = bordCell.AreaCard;
                        pairList = (new(card, power));
                    }
                    break;
            }
            return (bordCell, pairList);
        }

        // セルを取得
        public GameBoardCell GetCell(int row, int col)
        {
            if (row >= 0 && row < Grid.GetLength(0) && col >= 0 && col < Grid.GetLength(1))
            {
                return Grid[row, col];
            }
            return null; // 範囲外の場合はヌルを返す
        }
    }
}
