using System.Collections;
using System.Collections.Generic;
using TripleTriad.Cards;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace TripleTriad.MyEditor
{
    [CustomEditor(typeof(PlayableCard))]
    public class PlayableCardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // カスタムスタイルの作成
            GUIStyle largeFontStyle = new GUIStyle(EditorStyles.label);
            largeFontStyle.fontSize = 18;
            largeFontStyle.fontStyle = FontStyle.Bold;
            largeFontStyle.normal.textColor = Color.cyan;

            serializedObject.Update();

            PlayableCard card = (PlayableCard)target;

            // 力の値を表示
            EditorGUILayout.LabelField("上の力の値", card.TopPower.ToString(), largeFontStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("下の力の値", card.BottomPower.ToString(), largeFontStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("左の力の値", card.LeftPower.ToString(), largeFontStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("右の力の値", card.RightPower.ToString(), largeFontStyle);
            EditorGUILayout.Space();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
