using System.Collections.Generic;
using TripleTriad.Area;
using TripleTriad.Bord;
using UnityEngine;
using UnityEngine.EventSystems;
using static TripleTriad.Cards.PlayableCardState;

namespace TripleTriad.Cards
{
    /// <summary>
    /// ゲーム中に使用するカードのクラス
    /// </summary>
    [RequireComponent(typeof(DragAndDropImage))]
    public class PlayableCard : CardBase, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
    {
        //--------------------
        // カードの状態
        public PlayableCardState currentState;
        public PlayableCardState InHandState { get; private set; }
        public PlayableCardState OnGridState { get; private set; }
        public PlayableCardState SelectedState { get; private set; }
        public PlayableCardState SetState { get; private set; }
        public void TransitionToState(PlayableCardState newState)
        {
            currentState?.ExitState();
            currentState = newState;
            currentState?.EnterState();
        }
        //--------------------

        //--------------------
        // パワーアップする力の値
        const int Plus_Power = 3;
        // 上の力の値
        int topPower;
        public int TopPower { get { return topPower; } private set { topPower = value; } }
        // 下の力の値
        int bottomPower;
        public int BottomPower { get { return bottomPower; } private set { bottomPower = value; } }
        // 左の力の値
        int leftPower;
        public int LeftPower { get { return leftPower; } private set { leftPower = value; } }
        // 右の力の値
        int rightPower;
        public int RightPower { get { return rightPower; } private set { rightPower = value; } }
        //--------------------

        //--------------------
        private RectTransform rectCardTransform;
        const float Selected_Position_Z = -200; // カードを選択している状態の時のZ座標
        public float GetSelectedPositionZ => Selected_Position_Z;
        const float DeSelect_Position_Z = -100; // カードを選択していない状態の時のZ座標
        public float GetDeselectPositionZ => DeSelect_Position_Z;
        //--------------------

        //--------------------
        // 重なり判定用
        DragAndDropImage dragAndDropImage;
        bool cardMove;
        public bool CardMove
        {
            get => dragAndDropImage.isDragging;
            set
            {
                dragAndDropImage.isDragging = value;
            }
        }
        [Header("重なり判定用のRectTransform")]
        [SerializeField] RectTransform rectOverlapTransform;
        public RectTransform RectOverlapTransform => rectOverlapTransform;
        //--------------------

        //--------------------
        // 手札の配置セル
        GameHandCell handCell;
        public GameHandCell HandCell { get => handCell; set => handCell = value; }
        // 盤面の配置セル
        public GameBoardCell ThisCardCell { get; set; }
        //--------------------
        public AudioClip pointerEnterClip;
        public AudioClip selectedClip;
        public AudioClip setClip;

        private void Start()
        {
            InitializeCardSettings();

            // 状態インスタンスの初期化
            InHandState = new InHandState(this);
            SelectedState = new SelectedState(this);
            SetState = new SetState(this);

            // 初期状態の設定
            TransitionToState(InHandState);
        }

        internal bool MyTurn()
        {
            if (TripleTriadGameSystem.instance.CurrentTurn
                == TripleTriadGameSystem.instance.TurnOwnerData[CardCurrentOwner])
            {
                return CardMove = true;
            }
            else
            {
                return CardMove = false;
            }
        }

        // プレイアブルカードの設定を行うメソッド
        public void InitializeCardSettings()
        {
            // カードのフレームを変更
            DisplayCard();

            // カードのコンポーネントの設定
            dragAndDropImage = GetComponent<DragAndDropImage>();
            rectCardTransform = GetComponent<RectTransform>();

            // カードの力の値を設定
            TopPower = Card.GetTopPower;
            BottomPower = Card.GetBottomPower;
            LeftPower = Card.GetLeftPower;
            RightPower = Card.GetRightPower;
        }

        // カードのパワーを上げて新しい値を返す
        public int PlusPower(DirectionType directionType, bool textOn = true)
        {
            // DirectionType に対応する Power と Text を取得
            var data = directionData[directionType];

            // 新しいパワーを計算
            int newPower = Mathf.Min(data.power + Plus_Power, 9);

            // Text に新しいパワーを表示
            if (textOn) data.powerText.text = newPower.ToString();

            return newPower;
        }

        /// <summary>
        /// 特定の方向のセルとカードデータとパワーをまとめたディクショナリーを返す
        /// </summary>
        /// <param name="direction">方向</param>
        public (CardPowerPair, GameBoardCell) CheckBordCellCard(DirectionType direction)
        {
            if (ThisCardCell != null)
            {
                int cellRow = ThisCardCell.GridRow;
                int cellCol = ThisCardCell.GridColumn;

                // direction方向にある相手カードの情報を調べて返す
                GameBoardCell targetCell = GameBoardGrid.instance.CheckDirectionCardData(direction, cellRow, cellCol).Item1;

                CardPowerPair cardPowerPair = GameBoardGrid.instance.CheckDirectionCardData(direction, cellRow, cellCol).Item2;

                // カードがあった場合は勝負
                if (targetCell != null && cardPowerPair != null)
                {
                    //弱点を突いているかどうか
                    if (CheckCardElementTypeMatchup(cardData, cardPowerPair.PlayableCard.Card)
                        && CardCurrentOwner != cardPowerPair.PlayableCard.CardCurrentOwner)
                    {
                        PlusPower(direction);
                    };
                    return (cardPowerPair, targetCell);
                }
            }
            return (null, null);
        }

        /// <summary>
        /// バトルを行う
        /// </summary>
        /// <param name="direction">方向</param>
        public bool Battle(DirectionType direction)
        {
            if (ThisCardCell != null)
            {
                if (CheckBordCellCard(direction).Item1 != null)
                {
                    var cardDataPowerPair = CheckBordCellCard(direction).Item1;
                    var targetPlayeableCard = cardDataPowerPair.PlayableCard;

                    if (CardCurrentOwner != targetPlayeableCard.CardCurrentOwner)
                    {
                        var cardPower = directionData[direction].power;

                        if (CheckCardElementTypeMatchup(cardData, cardDataPowerPair.PlayableCard.Card))
                        {
                            cardPower = PlusPower(direction);
                        };

                        // 勝っていた場合
                        if (cardPower > cardDataPowerPair.Power)
                        {
                            AddScore(1, true);
                            //Debug.Log($"<color=yellow>Player : {cardData.GetCardName}の攻撃力は{cardPower}</color>");
                            //Debug.Log($"<color=yellow>CPU : {cardDataPowerPair.PlayableCard.Card.GetCardName}の攻撃力は{cardDataPowerPair.Power}</color>");
                            //Debug.Log($"<color=yellow>Player : {cardData.GetCardName}は、CPU : {cardDataPowerPair.PlayableCard.Card.GetCardName}に勝利した</color>");
                            targetPlayeableCard.CardCurrentOwner = CardCurrentOwner;
                            targetPlayeableCard.ChangeCardFrame();
                            CheckBordCellCard(direction).Item2.ChangeCellImageColor(targetPlayeableCard);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        // 点数を追加する
        internal void AddScore(int score, bool minus = false)
        {
            if (CardCurrentOwner == CardOwnerType.Player)
            {
                TripleTriadGameSystem.instance.Player1Score = score;
                if (minus) TripleTriadGameSystem.instance.Player2Score = -score;
            }
            else
            {
                TripleTriadGameSystem.instance.Player2Score = score;
                if (minus) TripleTriadGameSystem.instance.Player1Score = -score;
            }
        }

        // カードのZ座標を変更する
        public void SetReactTransformPositionZ(float position)
        {
            Vector3 newPosition = rectCardTransform.localPosition;
            newPosition.z = position;
            rectCardTransform.localPosition = newPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (CardCurrentOwner != CardOwnerType.CPU) currentState.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CardCurrentOwner != CardOwnerType.CPU) currentState.OnPointerUp(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CardCurrentOwner != CardOwnerType.CPU) currentState.OnPointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CardCurrentOwner != CardOwnerType.CPU) currentState.OnPointerExit(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CardCurrentOwner != CardOwnerType.CPU) currentState.OnDrag(eventData);
        }
    }

    //--------------------
    /// <summary>
    /// PlayableCardとPowerをまとめたクラス
    /// </summary>
    public class CardPowerPair
    {
        public PlayableCard PlayableCard { get; set; }

        public int Power { get; set; }

        public CardPowerPair(PlayableCard playableCard, int power)
        {
            this.PlayableCard = playableCard;
            this.Power = power;
        }
    }

    //--------------------
    public abstract class PlayableCardState
    {
        protected PlayableCard card;

        public PlayableCardState(PlayableCard card)
        {
            this.card = card;
        }

        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);

        public abstract void OnDrag(PointerEventData eventData);

        public virtual void EnterState() { }
        public virtual void ExitState() { }

        /// <summary>
        /// ハンド状態
        /// </summary>
        public class InHandState : PlayableCardState
        {
            public InHandState(PlayableCard card) : base(card) { }

            public override void EnterState()
            {
                //GameManager.instance.SelectedCardArea = null;
                card.SetReactTransformPositionZ(card.GetDeselectPositionZ);
            }

            public override void OnPointerDown(PointerEventData eventData)
            {
                if (card.MyTurn())
                {
                    card.TransitionToState(card.SelectedState);
                }
            }

            public override void OnPointerUp(PointerEventData eventData) { }

            public override void OnDrag(PointerEventData eventData) { }

            public override void OnPointerEnter(PointerEventData eventData)
            {
                if (card.MyTurn() && GameManager.instance.SelectedPlayableCardData == null)
                {
                    card.SelectedOutlineImage.SetActive(true);
                    AudioManager.instance.PlayOneShotClip(card.pointerEnterClip);
                }
            }

            public override void OnPointerExit(PointerEventData eventData)
            {
                card.SelectedOutlineImage.SetActive(false);
            }
        }

        /// <summary>
        /// セレクト状態
        /// </summary>
        public class SelectedState : PlayableCardState
        {
            public SelectedState(PlayableCard card) : base(card) { }

            public override void EnterState()
            {
                card.SetReactTransformPositionZ(card.GetSelectedPositionZ);
                GameManager.instance.SelectedPlayableCardData = card;
                if (card.CardCurrentOwner == CardOwnerType.Player)
                    AudioManager.instance.PlayOneShotClip(card.selectedClip);
            }

            public override void ExitState()
            {
                card.SetReactTransformPositionZ(card.GetDeselectPositionZ);
                GameManager.instance.SelectedPlayableCardData = null;
            }

            public override void OnPointerEnter(PointerEventData eventData)
            {

            }

            public override void OnPointerExit(PointerEventData eventData) { }

            public override void OnPointerDown(PointerEventData eventData) { }

            public override void OnPointerUp(PointerEventData eventData)
            {

                if (card.ThisCardCell != null) // グリッド上で離した場合
                {
                    card.ThisCardCell.SetCardInArea(card.gameObject, card);
                    card.TransitionToState(card.SetState);

                }
                else // そうでない場合は手札に戻る
                {
                    card.HandCell.SetCardInArea(card.gameObject, card);
                    card.TransitionToState(card.InHandState);
                }
            }
            public override void OnDrag(PointerEventData eventData)
            {
                if (card.ThisCardCell != null)
                {
                    card.CheckBordCellCard(DirectionType.Top);
                    card.CheckBordCellCard(DirectionType.Left);
                    card.CheckBordCellCard(DirectionType.Bottom);
                    card.CheckBordCellCard(DirectionType.Right);
                }
                else
                {
                    card.SetDefaultPower();
                }

            }
        }

        /// <summary>
        /// セット状態
        /// </summary>
        public class SetState : PlayableCardState
        {
            public SetState(PlayableCard card) : base(card) { }

            public override void EnterState()
            {
                AudioManager.instance.PlayOneShotClip(card.setClip);
                card.HandCell.AreaCard = null;
                card.SelectedOutlineImage.SetActive(false);
                card.CardMove = false;
                card.AddScore(1);
                card.Battle(DirectionType.Top);
                card.Battle(DirectionType.Left);
                card.Battle(DirectionType.Bottom);
                card.Battle(DirectionType.Right);
                TripleTriadGameSystem.instance.ChangeState_TurnChange();
            }

            public override void ExitState() { }

            public override void OnPointerEnter(PointerEventData eventData)
            {
                //card.SelectedOutlineImage.SetActive(true);
            }

            public override void OnPointerExit(PointerEventData eventData)
            {
                //card.SelectedOutlineImage.SetActive(false)
            }

            public override void OnPointerDown(PointerEventData eventData) { }

            public override void OnPointerUp(PointerEventData eventData) { }

            public override void OnDrag(PointerEventData eventData) { }
        }
    }
}
