using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : TileEntity, IMoveable
    {
        private float MovementPollRate = 1/100f;
        public float MovementSpeed = .5f;
        private Blaster blaster;
        private float last_time = Time.time;

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
        }

        public override void DoTick()
        {
            base.DoTick();
        }

        void Start()
        {
            blaster = new Blaster(this);
            this.maxHealth = 10;
            this.Health = this.maxHealth;

            TimedActionManager.GetInstance().RegisterAction(
                () =>
                {
                    
                    if (last_time + MovementSpeed <= Time.time)
                    {
                        // Holding key down
                        last_time = Time.time;
                        HandleGetKey();
                    }
                    else
                    {
                        if (HandleGetKeyDown())
                        {
                            // Looks like we moved. Reset the clock!
                            last_time = Time.time;
                        }
                    }
                }
                , this, MovementPollRate
                );
        }

        private float movementCooldownTimer;
        private void HandleGetKey()
        {
            if (Input.GetKey(KeyCode.W))
            {
                DoMove(TileEntityConstants.DirectionVectors.UP);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                DoMove(TileEntityConstants.DirectionVectors.RIGHT);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                DoMove(TileEntityConstants.DirectionVectors.LEFT);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                DoMove(TileEntityConstants.DirectionVectors.DOWN);
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                this.blaster.DoShoot();
                //movementCooldownTimer = Cooldown_time;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                ClaimTile();
            }
        }

        /***
         * Okay, so this is a bit of a hack. We return true if
         * we moved, false otherwise. It's a hack because this 
         * function does more than checking movement so the logic
         * is kinda strange. Will refactor it later! That's what 
         * they all say....
         */
        private bool HandleGetKeyDown()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                DoMove(TileEntityConstants.DirectionVectors.UP);
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                DoMove(TileEntityConstants.DirectionVectors.RIGHT);
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                DoMove(TileEntityConstants.DirectionVectors.LEFT);
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                DoMove(TileEntityConstants.DirectionVectors.DOWN);
                return true;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                this.blaster.DoShoot();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                ClaimTile();
            }

            return false;
        }


        private void ClaimTile()
        {
            Debug.Log("Claim a tile");
            Tile.SIDE mySide = GetTile().GetSide();
            Vector2Int pos = gameBoard.GetTileEntityPosition(this);
            Tile tile = gameBoard.GetTile(pos.x, pos.y + (1 * (mySide == Tile.SIDE.LEFT ? 1 : -1)));

            if (tile != null)
            {
                this.ClaimTile(tile);
            }
        }
    }
}
