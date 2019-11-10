using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class GameBoard : MonoBehaviourPunCallbacks, IPunObservable
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


        public void Move(TileEntity tileEntity, MoveableTileEntity.Direction direction, EntityInfo entityInfo)
        {
            Vector2Int position;
            if (positionDict.TryGetValue(tileEntity.getUid(), out position))
            {
                Tile curTile = boardArray[position.x, position.y];

                switch (direction)
                {
                    case MoveableTileEntity.Direction.DOWN:
                        if (position.x < HEIGHT - 1)
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
                        if (position.y < WIDTH - 1)
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
                    if (positionDict.ContainsKey(tileEntity.getUid()))
                    {
                        positionDict[tileEntity.getUid()] = new Vector2Int(position.x, position.y);
                    }
                    else
                    {
                        positionDict.Add(tileEntity.getUid(), new Vector2Int(position.x, position.y));
                    }

                    // Already have info available! Update that
                    if (gameInfo.entityInfos.ContainsKey(entityInfo.uid))
                    {
                        EntityInfo inf;
                        gameInfo.entityInfos.TryGetValue(entityInfo.uid, out inf);
                        inf.position = position;
                        gameInfo.entityInfos[entityInfo.uid] = inf;
                    }
                    else
                    {
                        entityInfo.position = position;
                        // No info available. Use this one.
                        gameInfo.entityInfos.Add(entityInfo.uid, entityInfo);
                    }
                    gameInfo.wasUpdated = true;
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //if (stream.IsWriting)
            //{
            //    if (gameInfo.wasUpdated)
            //    {
            //        // We own this player: send the others our data
            //        string giString = gameInfo.Stringify();
            //        stream.SendNext(giString);
            //        gameInfo = new GameInfo();
            //    }
            //}
            //else
            //{
            //    // Network player, recieve data
            //    string r = (string)stream.ReceiveNext();
            //    GameInfo gi = GameInfo.fromString(r);
            //    ProcessNewGameInfo(gi);
            //}
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