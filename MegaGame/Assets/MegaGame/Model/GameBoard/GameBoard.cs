using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class GameBoard : MonoBehaviour
    {
        public static int HEIGHT = 3;
        public static int WIDTH = 6;
        
        private Dictionary<TileEntity, Vector2Int> positionDict = new Dictionary<TileEntity, Vector2Int>();
        private Tile[,] boardArray = new Tile[HEIGHT, WIDTH];
        // Start is called before the first frame update
        void Start()
        {

        }
        
        public void SetCellValue(int row, int column, Tile value)
        {
            boardArray[row, column] = value;
            Vector2Int position = new Vector2Int(row, column);

            foreach (TileEntity entity in value.GetEntities())
            {
                positionDict.Add(entity, position);
            }
        }

        public Tile GetTile(int row, int col)
        {
            return boardArray[row, col];
        }


        public void Move(TileEntity tileEntity, MoveableTileEntity.Direction direction, int distance)
        {
            Vector2Int position;
            if (positionDict.TryGetValue(tileEntity, out position))
            {
                Tile curTile = boardArray[position.x, position.y];

                switch (direction)
                {
                    case MoveableTileEntity.Direction.DOWN:
                        if (position.x < HEIGHT)
                        {
                            ++position.x;
                        }
                        break;

                    case MoveableTileEntity.Direction.UP:
                        if (position.x > 0)
                        {
                            --position.x;
                        }
                        break;

                    case MoveableTileEntity.Direction.LEFT:
                        if (position.y > 0)
                        {
                            --position.y;
                        }
                        break;

                    case MoveableTileEntity.Direction.RIGHT:
                        if (position.y < WIDTH)
                        {
                            ++position.y;
                        }
                        break;
                }

                Tile newTile = boardArray[position.x, position.y];

                if (newTile != curTile)
                {
                    // Remove from old tile
                    curTile.RemoveTileEntity(tileEntity);
                    // Place on new tile
                    newTile.AddTileEntity(tileEntity);
                    // Update the position
                    positionDict.Add(tileEntity, position);
                }
            }
            else
            {
                Debug.Log("[GameBoard] Failed to move tileEntity... not in dictionary!");
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < HEIGHT; ++i)
            {
                for (int j = 0; j < WIDTH; ++j)
                {
                    boardArray[i, j].DoTick();
                }
            }
        }
    }
}