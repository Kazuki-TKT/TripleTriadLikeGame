using System;
using System.Collections;
using System.Collections.Generic;
using TripleTriad.Bord;
using TripleTriad.Cards;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static TripleTriad.Cards.PlayableCard;

namespace TripleTriad.Area
{
    public enum GridColorType
    {
        White,
        Red,
        Blue,
        Yellow
    }

    public interface ICell
    {
        public int GridRow { get; }
        public int GridColumn { get; }
    }

    /// <summary>
    /// 対戦中のグリッドのクラス
    /// </summary>
    public class GameBoardCell : SetCardArea<PlayableCard>, ICell
    {
        //--------------------
        // 行
        [SerializeField] int gridRow;
        public int GridRow => gridRow;

        // 列
        [SerializeField] int gridCol;
        public int GridColumn => gridCol;
        //--------------------

        [SerializeField] Image cellImage;

        [SerializeField] RectTransform cellRectTransform;

        //--------------------
        GridColorType cellColor;
        public GridColorType SetCellColor
        {
            set
            {
                cellColor = value;
                switch (cellColor)
                {
                    case GridColorType.White:
                        cellImage.color = new Color32(255, 255, 255, 244);
                        break;
                    case GridColorType.Red:
                        cellImage.color = new Color32(255, 162, 148, 244);
                        break;
                    case GridColorType.Blue:
                        cellImage.color = new Color32(160, 148, 255, 244);
                        break;
                    case GridColorType.Yellow:
                        cellImage.color = new Color32(255, 237, 148, 244);
                        break;
                }
            }
        }
        //--------------------


        private void Awake()
        {
            if (SetArea != SetAreaType.Bord) SetArea = SetAreaType.Bord;
            if (cellRectTransform == null) Debug.LogWarning("girdRectTransformが設定されていません");
            if (cellImage == null) cellImage = GetComponent<Image>();
        }

        void Start()
        {
            GameBoardGrid.instance.Grid[GridRow, GridColumn] = this;
            areaAction += OnAreaAction;
        }

        private void Update()
        {
            // プレイアブルカードがNULLならリターン
            // エリアにカードが設定されているならリターン
            if (GameManager.instance.SelectedPlayableCardData == null) return;

            var playableCard = GameManager.instance.SelectedPlayableCardData;

            if (onArea == false)
            {
                // プレイアブルカードと重なっているかどうか
                bool isOverLapping = cellRectTransform.IsOverlapping(playableCard.RectOverlapTransform);
                if (isOverLapping && AreaCard != playableCard)
                {
                    SetCellColor = GridColorType.Yellow;
                    AreaCard = playableCard;
                    AreaCard.ThisCardCell = this;
                    return;
                }
                else if (!isOverLapping && AreaCard == playableCard)
                {
                    if (cellColor != GridColorType.White) SetCellColor = GridColorType.White;
                    if (AreaCard != null)
                    {
                        AreaCard.SetDefaultPower();
                    }
                    AreaCard.ThisCardCell = null;
                    AreaCard = null;
                }
            }
        }

        // カードが設置された時に行う関数
        public void OnAreaAction(GameObject targetObject)
        {
            onArea = PlaceCard(targetObject);
            AreaCard = GameManager.instance.SelectedPlayableCardData;
            if (AreaCard != null)
            {
                AreaCard.ThisCardCell = this;
                ChangeCellImageColor(AreaCard);
            }
        }

        // セルのカラーを変更
        public void ChangeCellImageColor(PlayableCard playableCard)
        {
            switch (playableCard.CardCurrentOwner)
            {
                case CardOwnerType.Player:
                    SetCellColor = GridColorType.Red;
                    break;
                case CardOwnerType.CPU:
                    SetCellColor = GridColorType.Blue;

                    break;
            }
        }
    }
}
