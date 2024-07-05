using System.Collections;
using System.Collections.Generic;
using TripleTriad.Cards;
using UnityEngine;
using static TripleTriad.Bord.GameBoardGrid;
using TripleTriad.Area;
using System;
using DG.Tweening;

namespace TripleTriad
{
    public class CpuAI : MonoBehaviour
    {
        public bool debug;
        // 盤面のカード
        [SerializeField] List<PlayableCard> boardCards;

        // CPU用のハンドグリッド
        [SerializeField] GameHandGrid cpuHand;

        // CPU用のハンドリスト
        [SerializeField] List<PlayableCard> cpuCards;

        // 使用可能なグリッド
        public GameBoardCell[,] AvailableGrid { get; private set; }

        /// <summary>
        /// 置くことができるカードのリスト
        /// GameBordCell = 置くことができるセル
        /// Tuple<DirectionType,CardPowerPair>　= 特定の方向、特定の方向にあるカードとそのPowerをまとめたクラス
        /// </summary>
        List<KeyValuePair<GameBoardCell, Tuple<DirectionType, CardPowerPair>>> placeableCards;

        public IEnumerator PlayCPU()
        {
            yield return new WaitForSeconds(0.5f);
            UpdateCurrentBoardAndHandCards();
            ProcessCpuCardPlacement();
        }

        private void UpdateCurrentBoardAndHandCards()
        {
            instance.GetCurrentBoardCard();// 現在の盤面にあるカードを取得
            if (instance.currentCardsData != null) boardCards = instance.currentCardsData;
            cpuCards = cpuHand.GetCurrentHandCards();
        }

        void ProcessCpuCardPlacement()
        {
            List<GameBoardCell> availableCells = GetAvailableCells();

            placeableCards = new List<KeyValuePair<GameBoardCell, Tuple<DirectionType, CardPowerPair>>>();

            // 各セルに対して各方向をチェックし、必要に応じてリストに追加
            foreach (GameBoardCell cell in availableCells)
            {
                AddToDictionaryIfValid(DirectionType.Top, cell);
                AddToDictionaryIfValid(DirectionType.Bottom, cell);
                AddToDictionaryIfValid(DirectionType.Left, cell);
                AddToDictionaryIfValid(DirectionType.Right, cell);
            }

            if (placeableCards.Count > 0)
            {
                if (TryFindWinningMoves(out var winningMoves))
                {
                    var (cell, card) = GetRandomCellCardPair(winningMoves);
                    MoveCpuCard(cell, card);
                }
                else
                {
                    var cell = GetRandomCell(availableCells);
                    var card = GetRandomCard();
                    MoveCpuCard(cell, card);
                }
            }
            else
            {
                var cell = GetRandomCell(availableCells);
                var card = GetRandomCard();
                MoveCpuCard(cell, card);
            }
        }

        // データをチェックし、リストに追加する関数
        void AddToDictionaryIfValid(DirectionType direction, GameBoardCell cell)
        {
            var data = instance.CheckDirectionCardData(direction, cell.GridRow, cell.GridColumn).Item2;
            if (data != null)
            {
                placeableCards.Add(new KeyValuePair<GameBoardCell, Tuple<DirectionType, CardPowerPair>>(cell, Tuple.Create(direction, data)));
            }
        }

        /// <summary>
        /// 勝利する可能性を探す
        /// </summary>
        /// <param name="winningMoves">勝利するパターンのセルとカードをまとめたリスト</param>
        /// <returns>勝利する移動が見つかったかどうか</returns>
        private bool TryFindWinningMoves(out List<KeyValuePair<GameBoardCell, PlayableCard>> winningMoves)
        {
            winningMoves = new List<KeyValuePair<GameBoardCell, PlayableCard>>();
            bool foundWinningMove = false;

            // すべてのプレース可能なカードに対してループ
            foreach (var placeableCard in placeableCards)
            {
                // 勝利するかどうかをチェック
                foreach (var cpuCard in cpuCards)
                {
                    if (IsWinningMove(placeableCard, cpuCard))
                    {
                        winningMoves.Add(new KeyValuePair<GameBoardCell, PlayableCard>(placeableCard.Key, cpuCard));
                        foundWinningMove = true;
                    }
                }
            }

            return foundWinningMove;
        }


        private bool IsWinningMove(KeyValuePair<GameBoardCell, Tuple<DirectionType, CardPowerPair>> placeableCard, PlayableCard cpuCard)
        {
            // プレース可能なカードがCPUの所有者の場合、勝利しないと判断
            if (placeableCard.Value.Item2.PlayableCard.CardCurrentOwner == CardOwnerType.CPU) return false;

            // CPUカードの特定の方向のパワーを取得します
            int cpuCardPower = cpuCard.DirectionData[placeableCard.Value.Item1].power;

            // 弱点をついているかどうか、かつプレイヤーカードの所有者がプレイヤーの場合
            if (cpuCard.CheckCardElementTypeMatchup(cpuCard.Card, placeableCard.Value.Item2.PlayableCard.Card) &&
                placeableCard.Value.Item2.PlayableCard.CardCurrentOwner == CardOwnerType.Player)
            {
                // パワーを上げる
                cpuCardPower = cpuCard.PlusPower(placeableCard.Value.Item1, false);
            }

            // CPUカードのパワーがプレース可能なカードのパワーを超えている場合、勝利すると判断
            return cpuCardPower > placeableCard.Value.Item2.Power;
        }

        // 使用可能なセルのリストを返す
        List<GameBoardCell> GetAvailableCells()
        {
            AvailableGrid = instance.Grid;
            List<GameBoardCell> cellList = new List<GameBoardCell>();

            // 多次元配列をリストに変更
            for (int row = 0; row < AvailableGrid.GetLength(0); row++)
            {
                for (int col = 0; col < AvailableGrid.GetLength(1); col++)
                {
                    cellList.Add(AvailableGrid[row, col]);
                }
            }
            // リストから盤面のカードを削除　⇒　配置可能なセルのリストの作成
            foreach (PlayableCard card in boardCards)
            {
                if (ContainsCell(card.ThisCardCell))
                {
                    cellList.Remove(card.ThisCardCell);
                }
            }
            return cellList;
        }

        bool ContainsCell(GameBoardCell targetCell)
        {
            for (int row = 0; row < instance.Grid.GetLength(0); row++)
            {
                for (int col = 0; col < instance.Grid.GetLength(1); col++)
                {
                    if (instance.Grid[row, col] == targetCell)
                    {
                        return true; // 見つかった場合はtrueを返す
                    }
                }
            }
            return false; // 見つからなかった場合はfalseを返す
        }

        // CPUのカードを動かす
        void MoveCpuCard(GameBoardCell targetCell, PlayableCard targetCard)
        {
            targetCard.TransitionToState(targetCard.SelectedState);
            Vector3 targetPosition = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y);
            targetCard.transform.DOMove(targetPosition, 0.3f).OnComplete(() =>
            {
                targetCard.ThisCardCell.SetCardInArea(targetCard.gameObject, targetCard);
                targetCard.TransitionToState(targetCard.SetState);
            });
        }

        PlayableCard GetRandomCard()
        {
            if (cpuCards == null || cpuCards.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, cpuCards.Count);
            return cpuCards[randomIndex];
        }

        GameBoardCell GetRandomCell(List<GameBoardCell> cells)
        {
            if (cells == null || cells.Count == 0)
            {
                return null;
            }
            int randomIndex = UnityEngine.Random.Range(0, cpuCards.Count);
            return cells[randomIndex];
        }

        (GameBoardCell, PlayableCard) GetRandomCellCardPair(List<KeyValuePair<GameBoardCell, PlayableCard>> cellCardPair)
        {
            if (cellCardPair == null || cellCardPair.Count == 0)
            {
                return (null, null);
            }

            int randomIndex = UnityEngine.Random.Range(0, cellCardPair.Count);
            return (cellCardPair[randomIndex].Key, cellCardPair[randomIndex].Value);
        }
    }
}
