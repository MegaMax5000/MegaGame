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
        [SerializeField]
        private Tile[,] boardArray = new Tile[HEIGHT, WIDTH];

        public List<GameObject> initTileList;
        // Start is called before the first frame update
        void Start()
        {
            for (int row = 0; row < HEIGHT; ++row)
            {
                for (int col = 0; col < WIDTH; ++col)
                {
                    int indx = row * HEIGHT + col;
                    
                    if (col < WIDTH/2)
                    {
                        Tile.SIDE side = Tile.SIDE.LEFT;
                        Tile tile = new NeutralTile(this, side);
                        SetTileValue(row, col, tile);
                    }
                }
            }
        }
        
        public void SetTileValue(int row, int column, Tile value)
        {
            boardArray[row, column] = value;
            Vector2Int position = new Vector2Int(row, column);

            foreach (TileEntity entity in value.GetEntities())
            {
                positionDict.Add(entity, position);
            }
        }

        public void AddEntityToTile(int row, int column, TileEntity tileEntity)
        {
            if (row > HEIGHT || column > WIDTH || row < 0 || column < 0)
            {
                Debug.Log("[GameBoard] Cannot add entity to tile: (" + row + "," + column + ")");
                return;
            }
            boardArray[row, column].AddEntity(tileEntity);
        }

        public Tile GetTile(int row, int col)
        {
            return boardArray[row, col];
        }


        public void Move(TileEntity tileEntity, MoveableTileEntity.Direction direction)
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