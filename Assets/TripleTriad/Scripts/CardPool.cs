using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleTriad.Cards
{
    /// <summary>
    /// カードプール用のクラス
    /// </summary>
    [System.Serializable]
    public class CardPool
    {
        [SerializeField] List<CardData> cardPool;
        public List<CardData> CardPoolList { get => cardPool; }
    }
}
