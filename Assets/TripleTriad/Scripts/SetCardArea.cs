using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TripleTriad.Cards;
using UnityEngine.Events;

namespace TripleTriad.Area
{
    public enum SetAreaType
    {
        Hand,
        Bord,
    }

    /// <summary>
    /// カードを配置することができるオブジェクトにスクリプトに継承させるクラス
    /// </summary>
    public class SetCardArea<T> : MonoBehaviour where T : ICardHolder
    {
        public bool onArea = false;

        //--------------------
        SetAreaType setArea;
        public SetAreaType SetArea { get => setArea; set => setArea = value; }
        //--------------------

        //--------------------
        [SerializeField] protected T areaCard;
        public T AreaCard { get => areaCard; set => areaCard = value; }
        //--------------------

        //--------------------
        public UnityAction<GameObject> areaAction;
        //--------------------

        public void SetCardInArea(GameObject targetObject, T cardData)
        {
            areaCard = cardData;
            areaAction?.Invoke(targetObject);
        }

        // カードを指定位置に配置するメソッド
        public bool PlaceCard(GameObject targetObject)
        {
            if (targetObject.TryGetComponent(out RectTransform rectTransform))
            {
                rectTransform.position = gameObject.transform.position;
            }
            //Debug.Log($"<color=pink>{areaCard.Card.GetCardName} をセットしました</color>");
            return true;
        }
    }
}