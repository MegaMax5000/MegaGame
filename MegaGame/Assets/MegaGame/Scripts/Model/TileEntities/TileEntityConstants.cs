using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public static class TileEntityConstants
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public static class DirectionVectors
        {
            public static Vector2Int UP = new Vector2Int(-1, 0);

            public static Vector2Int DOWN = new Vector2Int(1, 0);

            public static Vector2Int LEFT = new Vector2Int(0, -1);

            public static Vector2Int RIGHT = new Vector2Int(0, 1);

        }

        public static Vector2Int UNDEFINED_POSITION = new Vector2Int(-1, -1);
    }
}