using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class GameBoard : MonoBehaviourPunCallbacks
    {
        public static int HEIGHT = 3;
        public static int WIDTH = 6;

        protected GameInfo gameInfo = new GameInfo();
        public GameInfo GetGameInfo() { return gameInfo; }

        private Dictionary<string, Vector2Int> positionDict = new Dictionary<string, Vector2Int>();
        [SerializeField]
        private Tile[,] boardArray = new Tile[HEIGHT, WIDTH];

        public List<GameObject> initTileList;

        // Start is called before the first frame update
        void Awake()
        {
            InitTiles();
        }

        private void InitTiles()
        {
            foreach (var tile in initTileList)
            {
                string name = tile.name;
                int row = (int)(name[4] - '0');
                int column = (int)(name[5] - '0');

                Tile t = tile.GetComponent<NeutralTile>();

                Tile.SIDE side = Tile.SIDE.RIGHT;
                if (column < WIDTH / 2)
                {
                    side = Tile.SIDE.LEFT;
                }
                t.SetSide(side);

                t.gameBoard = this;
                SetTileValue(row, column, t);
            }
            //for (int row = 0; row < HEIGHT; ++row)
            //{
            //    for (int col = 0; col < WIDTH; ++col)
            //    {
            //        int index = row * HEIGHT + col;

            //        Tile.SIDE side = Tile.SIDE.RIGHT;
            //        if (col < WIDTH / 2)
            //        {
            //            side = Tile.SIDE.LEFT;
            //        }
            //        Tile tile = initTileList[index].GetComponent<NeutralTile>();
            //        tile.SetSide(side);
            //        tile.gameBoard = this;
            //        SetTileValue(row, col, tile);
            //    }
            //}
        }

        public void SetTileValue(int row, int column, Tile value)
        {
            boardArray[row, column] = value;
            Vector2Int position = new Vector2Int(row, column);

            foreach (TileEntity entity in value.GetEntities())
            {
                if (positionDict.ContainsKey(entity.getUid()))
                {
                    positionDict[entity.getUid()] = position;
                }
                else
                {
                    positionDict.Add(entity.getUid(), position);
                }
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

            if (positionDict.ContainsKey(tileEntity.getUid()))
            {
                positionDict[tileEntity.getUid()] = new Vector2Int(row, column);
            }
            else
            {
                positionDict.Add(tileEntity.getUid(), new Vector2Int(row, column));
            }
        }

        public Tile GetTile(int row, int col)
        {
            return boardArray[row, col];
        }

        public Tile GetTile(Vector2Int coords)
        {
            return boardArray[coords.x, coords.y];
        }

        private bool UpdateOrAddToPositionDictionary(TileEntity te, Vector2Int value)
        {
            return UpdateOrAddToPositionDictionary(te.getUid(), value);
        }

        // Will return true if added a new entry, false if updated existing one
        private bool UpdateOrAddToPositionDictionary(string key, Vector2Int value)
        {
            // Update the position
            if (positionDict.ContainsKey(key))
            {
                positionDict[key] = value;
                return false;
            }
            else
            {
                positionDict.Add(key, value);
                return true;
            }
        }

        private void UpdateTileEntityPosition(TileEntity tileEntity, Vector2Int from, Vector2Int to)
        {
            Tile fromTile = GetTile(from);
            Tile toTile = GetTile(to);

            if (fromTile.Equals(toTile))
            {
                // Nothing to do
                return; 
            }

            fromTile.RemoveTileEntity(tileEntity);
            // Place on new tile
            toTile.AddTileEntity(tileEntity);

            // Update the entity's position
            UpdateOrAddToPositionDictionary(tileEntity, to);

            // Get or create EntityInfo
            EntityInfo info = gameInfo.GetEntityInfoOrDefault(tileEntity.getUid(), new EntityInfo(tileEntity.getUid()));
            info.position = to;
            gameInfo.UpdateOrAddToEntityInfoDictionary(info);
        }

        public void Move(TileEntity tileEntity, TileEntityConstants.Direction direction)
        {
            Vector2Int curPosition;
            if (positionDict.TryGetValue(tileEntity.getUid(), out curPosition))
            {
                Vector2Int newPosition = UpdatePosition(curPosition, direction);
                UpdateTileEntityPosition(tileEntity, curPosition, newPosition);
            }
            else
            {
                Debug.Log("[GameBoard] Failed to move tileEntity... not in dictionary!");
            }
        }

        private Vector2Int UpdatePosition(Vector2Int position, TileEntityConstants.Direction direction)
        {
            Tile curTile = GetTile(position); 
            Tile.SIDE side = curTile.GetSide();
            switch (direction)
            {
                case TileEntityConstants.Direction.DOWN:
                    if (position.x < HEIGHT - 1 && GetTile(position.x, position.y).GetSide().Equals(side))
                    {
                        ++position.x;
                    }
                    break;

                case TileEntityConstants.Direction.UP:
                    if (position.x > 0 && GetTile(position.x - 1, position.y).GetSide().Equals(side))
                    {
                        --position.x;
                    }
                    break;

                case TileEntityConstants.Direction.LEFT:
                    if (position.y > 0 && GetTile(position.x, position.y - 1).GetSide().Equals(side))
                    {
                        --position.y;
                    }
                    break;

                case TileEntityConstants.Direction.RIGHT:
                    if (position.y < WIDTH - 1 && GetTile(position.x, position.y + 1).GetSide().Equals(side))
                    {
                        ++position.y;
                    }
                    break;
            }
            return position;
        }
        // Update is called once per frame
        void Update()
        {
            TickAllTiles();
        }

        private void TickAllTiles()
        {
            for (int i = 0; i < HEIGHT; ++i)
            {
                for (int j = 0; j < WIDTH; ++j)
                {
                    boardArray[i, j].DoTick();
                }
            }
        }

        public void ProcessNewGameInfo(GameInfo info)
        {
            foreach (EntityInfo i in info.entityInfos.Values)
            {
                string uid = i.uid;
                
                if (positionDict.ContainsKey(uid))
                {
                    Vector2Int oldPos = positionDict[uid];
                    Vector2Int newPos = i.position;

                    if (oldPos != newPos)
                    {
                        positionDict[uid] = newPos;

                        Tile newTile = GetTile(newPos.x, newPos.y);
                        Tile oldTile = GetTile(oldPos.x, oldPos.y);

                        newTile.AddTileEntity(oldTile.GetEntity(uid));
                        oldTile.RemoveTileEntity(uid);
                    }
                }
                else
                {
                    PlayerTileEntity[] players = GameObject.FindObjectsOfType<PlayerTileEntity>();
                    foreach (var p in players)
                    {
                        if (p.getUid() == uid)
                        {
                            AddEntityToTile(i.position.x, i.position.y, p);
                        }
                    }
                }
            }
        }
    }
}