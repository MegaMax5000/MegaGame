using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class NeutralTile : Tile
    {

        public NeutralTile(GameBoard gb, SIDE side) : base(gb, side)
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            entityOffset = new Vector3(0, 1.5f, 0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void ApplyEffect(TileEntity tileEntity)
        {

        }

        public override void DoTick()
        {
            base.DoTick();
        }
    }
}