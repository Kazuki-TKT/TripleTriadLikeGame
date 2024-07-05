using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleTriad
{
    [CreateAssetMenu(fileName = "NewElementDatas", menuName = "Triple Triad/Symbol")]
    public class ElementSymbolData : ScriptableObject
    {
        [SerializeField] Sprite fire;
        public Sprite FireSprite => fire;
        [SerializeField] Sprite water;
        public Sprite WaterSprite => water;
        [SerializeField] Sprite grass;
        public Sprite GrassSprite => grass;
        [SerializeField] Sprite light;
        public Sprite LightSprite => light;
        [SerializeField] Sprite darkness;
        public Sprite DarknessSprite => darkness;
        [SerializeField] Sprite neutral;
        public Sprite NeutralSprite => neutral;
    }
}
