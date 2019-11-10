using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : MoveableTileEntity, IPunObservable
    {
        public float Cooldown_time = 0.1f;

        //never getting called right now
        public PlayerTileEntity(GameBoard gb, string name, float maxHealth, bool isPlayer1) : base(gb, name, maxHealth)
        {
            if (isPlayer1)
            {
                this.uid = "PLAYER_1";
            }
            else
            {
                this.uid = "PLAYER_2";
            }
        }

        public override void DoMove(Direction direction, EntityInfo info)
        {
            GameManager.Instance.MyGameBoard.Move(this, direction, info);
            cooldownTimer = Cooldown_time;
        }

        public override void DoTick()
        {
            // Do nothing
        }

        private float cooldownTimer;
        // Update is called once per frame
        void Update()
        {
            EntityInfo entityInfo = new EntityInfo(this.getUid());
            if (cooldownTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    DoMove(Direction.UP, entityInfo);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DoMove(Direction.RIGHT, entityInfo);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    DoMove(Direction.LEFT, entityInfo);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    DoMove(Direction.DOWN, entityInfo);
                }
            }
            else
            {
                cooldownTimer -= Time.deltaTime;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(uid);
            }
            else
            {
                // Network player, recieve data
                setUid((string)stream.ReceiveNext());
            }
        }
    }
}
