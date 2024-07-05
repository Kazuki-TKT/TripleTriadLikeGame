using System.Collections;
using System.Collections.Generic;
using TripleTriad.Cards;
using UnityEngine;
using TMPro;
using State = StateMachine<TripleTriad.TripleTriadGameSystem>.State;
using UnityEngine.SceneManagement;

namespace TripleTriad
{
    // ターンの状態
    public enum TurnState
    {
        Player1,
        Player2,
        None,
    }

    public class TripleTriadGameSystem : MonoBehaviour
    {
        public static TripleTriadGameSystem instance;

        private StateMachine<TripleTriadGameSystem> stateMachine;

        /// <summary>
        /// ゲームの状態を表す列挙型
        /// </summary>
        public enum PlayGameState : int
        {
            Preparing = 0, // ゲームの準備中
            Player1Turn = 1, // プレイヤー1のターン
            Player2Turn = 2, // プレイヤー2のターン
            TurnChange = 3, // ターンの切り替え中
            GameOver = 4 // ゲーム終了
        }

        bool gameOver = false; // ゲームオーバーかどうか

        //--------------------
        Dictionary<CardOwnerType, TurnState> turnOwnerData = new Dictionary<CardOwnerType, TurnState>()
        {
            {CardOwnerType.Player ,TurnState.Player1},
            {CardOwnerType.CPU ,TurnState.Player2}
        };
        public Dictionary<CardOwnerType, TurnState> TurnOwnerData => turnOwnerData;
        //--------------------
        [SerializeField] TurnState currentTurn;
        public TurnState CurrentTurn => currentTurn;
        //--------------------
        //--------------------
        // プレイヤー１のスコア類
        [SerializeField] TextMeshProUGUI player1ScoreText; // スコアを表示するテキストUI
        int player1Score = 0; // スコア
        public int Player1Score
        {
            get
            {
                return player1Score;
            }
            set
            {
                player1Score += value; // スコアを加算
                player1ScoreText.text = player1Score.ToString(); // UIに反映
            }
        }
        //--------------------
        // プレイヤー2のスコア類
        [SerializeField] TextMeshProUGUI player2ScoreText; // スコアを表示するテキストUI
        int player2Score = 0; // スコア
        public int Player2Score
        {
            get
            {
                return player2Score;
            }
            set
            {
                player2Score += value; // スコアを加算
                player2ScoreText.text = player2Score.ToString(); // UIに反映
            }
        }
        [SerializeField] CpuAI cpuAI; // CPU用のAI
        //--------------------
        [SerializeField] GameHandGrid player1HandGrid; // プレイヤー又はプレイヤー1の手札
        [SerializeField] GameHandGrid player2HandGrid; // CPU又はプレイヤー2の手札
        [SerializeField] GameCutInImage gameCutInImage; // カットインクラス
        [SerializeField] TextMeshProUGUI gameTurnText; // ゲームターンを表示するUI
        [SerializeField] GameObject gameOverObject; // ゲームオーバー用のオブジェクト

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            stateMachine = new StateMachine<TripleTriadGameSystem>(this);

            stateMachine.AddAnyTransition<Player1Turn>((int)PlayGameState.Player1Turn);
            stateMachine.AddAnyTransition<Player2Turn>((int)PlayGameState.Player2Turn);
            stateMachine.AddAnyTransition<TurnChange>((int)PlayGameState.TurnChange);
            stateMachine.AddAnyTransition<GameOver>((int)PlayGameState.GameOver);

            stateMachine.Start<Preparing>();
        }

        IEnumerator HandSettings(bool cpu = true)
        {
            // 初期状態
            gameOverObject.SetActive(false);
            gameOver = false;
            // デッキをセット
            yield return StartCoroutine(player1HandGrid.SetHandCoroutin(GameManager.instance.PlayerHand, CardOwnerType.Player));
            if (cpu)
            {
                yield return StartCoroutine(player2HandGrid.SetHandCoroutin(GameManager.instance.CpuHand, CardOwnerType.CPU));
            }
            // 乱数で先攻後攻を決める
            int min = 1;
            int max = 3;
            int random = Random.Range(min, max);
            // カットイン
            yield return StartCoroutine(gameCutInImage.PlayCutIn(GameCutInImage.SpriteType.Start));
            // プレイヤーのターン
            if (random != 1)
            {
                yield return StartCoroutine(gameCutInImage.PlayCutIn(GameCutInImage.SpriteType.YourTurn));
                stateMachine.Dispatch((int)PlayGameState.Player1Turn);
            }
            // CPUターン
            else
            {
                yield return StartCoroutine(gameCutInImage.PlayCutIn(GameCutInImage.SpriteType.EnemyTurn));
                stateMachine.Dispatch((int)PlayGameState.Player2Turn);
            }
        }

        // ターンを変更
        IEnumerator SwitchTurn(TurnState turnState)
        {
            // ターンNONEに変更
            currentTurn = TurnState.None;
            // 前のステートにより分岐
            if (turnState == TurnState.Player1) // プレイヤー１
            {
                yield return StartCoroutine(gameCutInImage.PlayCutIn(GameCutInImage.SpriteType.EnemyTurn));
                yield return new WaitForSeconds(0.5f);
                stateMachine.Dispatch((int)PlayGameState.Player2Turn);
            }
            else if (turnState == TurnState.Player2) // プレイヤー２
            {
                yield return StartCoroutine(gameCutInImage.PlayCutIn(GameCutInImage.SpriteType.YourTurn));
                yield return new WaitForSeconds(0.5f);
                stateMachine.Dispatch((int)PlayGameState.Player1Turn);
            }
        }

        // 外部からターンを変更するための関数
        public void ChangeState_TurnChange()
        {
            stateMachine.Dispatch((int)PlayGameState.TurnChange);
        }

        // ゲームオーバーかどうかをチェックする関数
        bool CheckGameOver()
        {
            // スコアが合計９かつゲームオーバーFalse状態の時
            if (player1Score + player2Score == 9 && gameOver == false)
            {
                stateMachine.Dispatch((int)PlayGameState.GameOver); // ゲームオーバーへ
                gameOver = true;
            }
            return gameOver;
        }

        // シーンをロード
        public void LoadGame()
        {
            SceneManager.LoadScene("PlayGameScene");
        }

        // 準備状態
        class Preparing : State
        {

            protected override void OnEnter(State prevState)
            {
                Owner.StartCoroutine(Owner.HandSettings());
                Owner.gameTurnText.text = "";
            }

            protected override void OnExit(State nextState)
            {

            }
        }

        // プレイヤー１
        class Player1Turn : State
        {
            protected override void OnEnter(State prevState)
            {
                Owner.currentTurn = TurnState.Player1;
                Owner.gameTurnText.text = "-PLAYER TURN-";
            }

            protected override void OnExit(State nextState)
            {

            }
        }

        // プレイヤー２
        class Player2Turn : State
        {
            protected override void OnEnter(State prevState)
            {
                Owner.currentTurn = TurnState.Player2;
                if (Owner.cpuAI != null)
                {
                    Owner.StartCoroutine(Owner.cpuAI.PlayCPU());
                    Owner.gameTurnText.text = "-CPU TURN-";
                }
            }

            protected override void OnExit(State nextState)
            {

            }
        }

        // ターンチェンジ
        class TurnChange : State
        {
            protected override void OnEnter(State prevState)
            {
                Owner.gameTurnText.text = "";
                if (!Owner.CheckGameOver())
                {
                    Owner.StartCoroutine(Owner.SwitchTurn(Owner.currentTurn));
                }
            }

            protected override void OnExit(State nextState)
            {

            }
        }

        // ゲームオーバー
        class GameOver : State
        {
            protected override void OnEnter(State prevState)
            {
                Owner.currentTurn = TurnState.None;
                Debug.Log("GameOver");
                Owner.gameTurnText.text = "-GAME OVER-";
                if (Owner.player1Score > Owner.player2Score)
                {
                    Owner.StartCoroutine(Owner.gameCutInImage.PlayResult(GameCutInImage.SpriteType.YouWin));
                }
                else
                {
                    Owner.StartCoroutine(Owner.gameCutInImage.PlayResult(GameCutInImage.SpriteType.YouLose));
                }
                Owner.gameOverObject.SetActive(true);
            }

            protected override void OnExit(State nextState)
            {

            }
        }
    }
}
