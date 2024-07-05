using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TripleTriad
{
    /// <summary>
    /// カードを動かすクラス
    /// </summary>
    public class DragAndDropImage : MonoBehaviour, IDragHandler
    {
        private RectTransform rectTransform;
        internal bool isDragging = false;

        private void Awake()
        {
            // RectTransformコンポーネントを取得
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // ドラッグ中は画像をマウスの位置に追従させる
            if (isDragging)
            {
                // マウス位置に追従するように座標を設定
                rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
            }
        }
    }
}
