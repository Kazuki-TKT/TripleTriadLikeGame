using System.Collections;
using System.Collections.Generic;
using TripleTriad.Area;
using TripleTriad.Cards;
using UnityEngine;

namespace TripleTriad.Bord
{
    public class GameHandCell : SetCardArea<PlayableCard>, ICell
    {
        //public bool created = false;

        [SerializeField] GameHandGrid gameHandGrid;

        //--------------------
        // 行
        [SerializeField] int gridRow;
        public int GridRow => gridRow;

        // 列
        [SerializeField] int gridCol;
        public int GridColumn => gridCol;

        [SerializeField] RectTransform cellRectTransform;
        //--------------------

        public RectTransform handCardRectTransform;

        void Start()
        {
            areaAction += OnAreaAction;
            if (gameHandGrid != null)
            {
                gameHandGrid.Grid[GridRow, GridColumn] = this;
                //created = true;
            }
        }

        public void OnAreaAction(GameObject targetObject)
        {
            onArea = PlaceCard(targetObject);
        }
    }
}
