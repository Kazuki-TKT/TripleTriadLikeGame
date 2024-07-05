using UnityEngine;
using UnityEditor;
using TripleTriad.Cards;

namespace TripleTriad.MyEditor
{
    [CustomEditor(typeof(CardData))]
    public class CardDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // ベースのインスペクタを表示
            base.OnInspectorGUI();

            // カスタムスタイルの作成
            GUIStyle largeFontStyle = new GUIStyle(EditorStyles.label);
            largeFontStyle.fontSize = 20;
            largeFontStyle.fontStyle = FontStyle.Bold;
            largeFontStyle.normal.textColor = Color.cyan;

            // カスタムエディタで使用するためのCardData参照
            CardData cardData = (CardData)target;

            if (string.IsNullOrEmpty(cardData.GetCardName))
            {
                EditorGUILayout.HelpBox("カード名が設定されていません。", MessageType.Warning);
            }

            if (cardData.GetCardSprite == null)
            {
                EditorGUILayout.HelpBox("カードイメージが設定されていません。", MessageType.Warning);
            }

            if (cardData.GetTopPower == 0)
            {
                EditorGUILayout.HelpBox("トップのパワーが設定されていません。", MessageType.Warning);
            }

            if (cardData.GetBottomPower == 0)
            {
                EditorGUILayout.HelpBox("ボトムのパワーが設定されていません。", MessageType.Warning);
            }

            if (cardData.GetLeftPower == 0)
            {
                EditorGUILayout.HelpBox("レフトのパワーが設定されていません。", MessageType.Warning);
            }

            if (cardData.GetRightPower == 0)
            {
                EditorGUILayout.HelpBox("ライトのパワーが設定されていません。", MessageType.Warning);
            }

            if (string.IsNullOrEmpty(cardData.GetCardFlavorText))
            {
                EditorGUILayout.HelpBox("フレーバーテキストが設定されていません。", MessageType.Warning);
            }

            // パワーの合計値を表示
            EditorGUILayout.Space(); // 見た目を良くするためにスペースを追加
            EditorGUILayout.LabelField("パワーの合計値", cardData.GetTotalPower.ToString(), largeFontStyle);

            switch (cardData.GetCardRank)
            {
                case CardRankType.OneStar:
                    if (cardData.GetTotalPower != 15)
                    {
                        EditorGUILayout.HelpBox($"パワーの合計値を15になるように設定してください", MessageType.Warning);
                    }
                    break;
                case CardRankType.TwoStar:
                    if (cardData.GetCardElement == ElementType.Neutral)
                    {
                        if (cardData.GetTotalPower != 18)
                        {
                            EditorGUILayout.HelpBox($"パワーの合計値を18になるように設定してください", MessageType.Warning);
                        }
                    }
                    else
                    {
                        if (cardData.GetTotalPower != 20)
                        {
                            EditorGUILayout.HelpBox($"パワーの合計値を20になるように設定してください", MessageType.Warning);
                        }
                    }
                    break;
                case CardRankType.ThreeStar:
                    if (cardData.GetTotalPower != 25)
                    {
                        EditorGUILayout.HelpBox($"パワーの合計値を25になるように設定してください", MessageType.Warning);
                    }
                    break;
            }
        }
    }
}

