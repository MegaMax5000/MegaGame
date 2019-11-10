using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class MoveableTileEntity : TileEntity
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public MoveableTileEntity(GameBoard gb, string name, float maxHealth) : base(gb, name, maxHealth)
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public abstract void DoMove(Direction direction, int distance);

    }
}