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
                if (positionDict.ContainsKey(entity.GetUid()))
                {
                    positionDict[entity.GetUid()] = position;
                }
                else
                {
                    positionDict.Add(entity.GetUid(), position);
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

            if (positionDict.ContainsKey(tileEntity.GetUid()))
            {
                positionDict[tileEntity.GetUid()] = new Vector2Int(row, column);
            }
            else
            {
                positionDict.Add(tileEntity.GetUid(), new Vector2Int(row, column));
            }
        }

        public Tile GetTile(int row, int col)
        {
            return boardArray[row, col];
        }

        // Returns entity's tile or null
        public Tile GetTile(TileEntity tileEntity)
        {
            Vector2Int position = GetTileEntityPosition(tileEntity);
            if (position == TileEntityConstants.UNDEFINED_POSITION)
            {
                return null;
            }
            else
            {
                return GetTile(position);
            }
        }

        public Vector2Int GetTileEntityPosition(TileEntity tileEntity)
        {
            if (positionDict.ContainsKey(tileEntity.GetUid()))
            {
                return positionDict[tileEntity.GetUid()];
            }
            else
            {
                return TileEntityConstants.UNDEFINED_POSITION;
            }
        }

        public Tile GetTile(Vector2Int coords)
        {
            return boardArray[coords.x, coords.y];
        }

        private bool UpdateOrAddToPositionDictionary(TileEntity te, Vector2Int value)
        {
            return UpdateOrAddToPositionDictionary(te.GetUid(), value);
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
            EntityInfo info = gameInfo.GetEntityInfoOrDefault(tileEntity.GetUid(), new EntityInfo(tileEntity.GetUid()));
            info.position = to;
            gameInfo.UpdateOrAddToEntityInfoDictionary(info);
        }

        public void Move(TileEntity tileEntity, Vector2Int direction)
        {
            Vector2Int curPosition;
            if (positionDict.TryGetValue(tileEntity.GetUid(), out curPosition))
            {
                Vector2Int newPosition = UpdatePosition(curPosition, direction);
                UpdateTileEntityPosition(tileEntity, curPosition, newPosition);
            }
            else
            {
                Debug.Log("[GameBoard] Failed to move tileEntity... not in dictionary!");
            }
        }

        public bool IsOnBoard(Vector2Int position)
        {
            if (position.x < 0 || position.x >= HEIGHT
                || position.y < 0 || position.y >= WIDTH)
            {
                return false;
            }
            return true;
        }
        private Vector2Int UpdatePosition(Vector2Int position, Vector2Int direction)
        {
            Tile curTile = GetTile(position); 
            Tile.SIDE side = curTile.GetSide();

            Vector2Int newPosition = position;
            newPosition += direction;

            // Make sure new position is still on the board
            if (!IsOnBoard(newPosition))
            {
                // not on board... do not update position
                return position;
            }
            

            Tile newTile = GetTile(newPosition);
            if (side != newTile.GetSide())
            {
                // we are not on the right side... revert!
                return position;
            }
            return newPosition;
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
                        if (p.GetUid() == uid)
                        {
                            AddEntityToTile(i.position.x, i.position.y, p);
                        }
                    }
                }
            }
        }
    }
}