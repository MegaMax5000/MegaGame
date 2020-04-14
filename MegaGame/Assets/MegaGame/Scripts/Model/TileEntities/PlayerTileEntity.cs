using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : TileEntity, IMoveable
    {
        public float Cooldown_time = 0.1f;

        private Blaster blaster;

        public Blaster GetBlaster()
        {
            return blaster;
        }
   
        //never getting called right now
        public PlayerTileEntity(GameBoard gb, string name, int maxHealth, bool isPlayer1) : base(gb, name, maxHealth, TileEntityType.Player)
        {
            
        }

        public void DoMove(Vector2Int direction)
        {
            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, direction);
            movementCooldownTimer = Cooldown_time;
        }

        public override void DoTick()
        {
            base.DoTick();
            HandleInput();
        }

        void Start()
        {
            blaster = new Blaster(this);
            this.maxHealth = 10;
            this.Health = this.maxHealth;
        }

        private float movementCooldownTimer;
        private void HandleInput()
        {
            if (movementCooldownTimer <= 0 && photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    DoMove(TileEntityConstants.DirectionVectors.UP);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DoMove(TileEntityConstants.DirectionVectors.RIGHT);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    DoMove(TileEntityConstants.DirectionVectors.LEFT);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    DoMove(TileEntityConstants.DirectionVectors.DOWN);
                }
                else if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    this.blaster.DoShoot();
                    //movementCooldownTimer = Cooldown_time;
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Claim a tile");
                    Tile.SIDE mySide = GetTile().GetSide();
                    Vector2Int pos = gameBoard.GetTileEntityPosition(this);
                    Tile tile = gameBoard.GetTile(pos.x, pos.y + (1 * (mySide == Tile.SIDE.LEFT ? 1 : -1)));

                    if (tile != null) {
                        this.ClaimTile(tile);
                    }
                }
            

            }
            else
            {
                movementCooldownTimer -= Time.deltaTime;
            }
        }
    }
}
