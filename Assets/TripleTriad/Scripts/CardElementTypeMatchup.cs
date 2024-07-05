using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleTriad.Cards
{
    /// <summary>
    /// 属性の相性を確かめるクラス
    /// </summary>
    public class CardElementTypeMatchup : MonoBehaviour
    {
        public bool CheckCardElementTypeMatchup(CardData userCard, CardData targetCard)
        {
            bool isMatched = false;
            switch (userCard.GetCardElement)
            {
                case ElementType.Neutral: // 無属性だった場合はFalse
                    break;
                case ElementType.Fire: // 火=>草
                    if (targetCard.GetCardElement == ElementType.Grass) isMatched = true;
                    break;
                case ElementType.Water: // 水=>火
                    if (targetCard.GetCardElement == ElementType.Fire) isMatched = true;
                    break;
                case ElementType.Grass: // 草=>水
                    if (targetCard.GetCardElement == ElementType.Water) isMatched = true;
                    break;
                case ElementType.Light: // 光=>闇
                    if (targetCard.GetCardElement == ElementType.Darkness) isMatched = true;
                    break;
                case ElementType.Darkness: // 闇=>光
                    if (targetCard.GetCardElement == ElementType.Light) isMatched = true;

                    break;
            }
            return isMatched;
        }
    }
}
