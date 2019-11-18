using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class GameBoard : MonoBehaviourPunCallbacks
    {
        public int GameBoardHeight { get { return height; } }
        public int GameBoardWidth { get { return width;  } }

        [SerializeField]
        private int height = 3;
        [SerializeField]
        private int width = 6;

        protected GameInfo gameInfo = new GameInfo();
        public GameInfo GetGameInfo() { return gameInfo; }

        private Dictionary<string, Vector2Int> positionDict = new Dictionary<string, Vector2Int>();
        [SerializeField]
        private Tile[,] boardArray;

        public List<GameObject> initTileList;

        // Start is called before the first frame update
        void Awake()
        {
            boardArray = new Tile[height, width];

            InitTiles();
        }

        private void InitTiles()
        {
            foreach (var tile in initTileList)
            {
                string name = tile.name;
                string coords = name.Split(' ')[1];

                string rowstring = coords.Split(',')[0];
                string columnstring = coords.Split(',')[1];

                int row = int.Parse(rowstring);
                int column = int.Parse(columnstring);

                Tile t = tile.GetComponent<NeutralTile>();

                Tile.SIDE side = Tile.SIDE.RIGHT;
                if (column < width / 2)
                {
                    side = Tile.SIDE.LEFT;
                }
                t.SetSide(side);

                t.gameBoard = this;
                SetTileValue(row, column, t);
            }
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
            if (row > height || column > width || row < 0 || column < 0)
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
            Debug.Log(gameInfo.ToString());
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
            if (position.x < 0 || position.x >= height
                || position.y < 0 || position.y >= width)
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
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
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