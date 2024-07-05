using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace TripleTriad
{
    /// <summary>
    /// ゲーム中のカットインを行うクラス
    /// </summary>
    public class GameCutInImage : MonoBehaviour
    {
        public enum SpriteType : int
        {
            Start = 0,
            YourTurn = 1,
            EnemyTurn = 2,
            YouWin = 3,
            YouLose = 4,
        }

        [SerializeField] GameObject cutInObject; // カットイン用のオブジェクト
        [SerializeField] Image cutInImage; // カットイン用のイメージ
        RectTransform rectTransform; // カットインのRectTransform

        //--------------------
        // カットイン用のポジション
        float start_X_Position; // START
        [SerializeField] float center_X_Position; // CENTER
        [SerializeField] float end_X_Position; // END

        //--------------------
        // カットイン用スプライト
        [SerializeField] Sprite startSprite;
        [SerializeField] Sprite yourTurnSprite;
        [SerializeField] Sprite enemyTurnSprite;
        [SerializeField] Sprite youWinSprite;
        [SerializeField] Sprite youLoseSprite;

        [SerializeField] AudioClip cutInClip; // カットイン用のSE

        // スプライトのタイプの列挙型とスプライト
        Dictionary<SpriteType, Sprite> SetSpriteTypeImage;
        private void Awake()
        {
            rectTransform = cutInObject.GetComponent<RectTransform>();
            start_X_Position = rectTransform.localPosition.x;
        }

        private void Start()
        {
            SetSpriteTypeImage = new Dictionary<SpriteType, Sprite>()
            {
                {SpriteType.Start,startSprite},
                {SpriteType.YourTurn,yourTurnSprite},
                {SpriteType.EnemyTurn,enemyTurnSprite},
                {SpriteType.YouWin, youWinSprite},
                {SpriteType.YouLose, youLoseSprite},
            };
        }

        // ターンチェンジ用カットイン
        public IEnumerator PlayCutIn(SpriteType spriteType)
        {
            cutInObject.SetActive(true);
            cutInImage.sprite = SetSpriteTypeImage[spriteType];

            yield return new WaitForSeconds(0.3f);

            AudioManager.instance.PlayOneShotClip(cutInClip);
            yield return rectTransform.DOLocalMoveX(center_X_Position, 0.2f).WaitForCompletion();

            yield return new WaitForSeconds(1f);

            yield return rectTransform.DOLocalMoveX(end_X_Position, 0.2f).WaitForCompletion();

            rectTransform.DOLocalMoveX(start_X_Position, 0f);
            cutInObject.SetActive(false);
        }

        // リザルト用カットイン
        public IEnumerator PlayResult(SpriteType spriteType)
        {
            AudioManager.instance.PlayOneShotClip(cutInClip);
            cutInObject.SetActive(true);
            cutInImage.sprite = SetSpriteTypeImage[spriteType];
            yield return rectTransform.DOLocalMoveX(center_X_Position, 0.5f);
        }
    }
}
