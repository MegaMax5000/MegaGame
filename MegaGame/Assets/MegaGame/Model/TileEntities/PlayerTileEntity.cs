using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : TileEntity, IMoveable, IPunObservable
    {
        public float Cooldown_time = 0.1f;

        //never getting called right now
        public PlayerTileEntity(GameBoard gb, string name, float maxHealth, bool isPlayer1) : base(gb, name, maxHealth)
        {
            
        }

        public void DoMove(TileEntityConstants.Direction direction)
        {
            GameManager.Instance.MyGameBoard.Move(this, direction);
            cooldownTimer = Cooldown_time;
        }

        public override void DoTick()
        {
            // Do nothing
        }

        // Update is called once per frame
        void Update()
        {
            handleMove();
        }

        private float cooldownTimer;
        private void handleMove()
        {
            if (cooldownTimer <= 0 && photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    DoMove(TileEntityConstants.Direction.UP);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    DoMove(TileEntityConstants.Direction.RIGHT);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    DoMove(TileEntityConstants.Direction.LEFT);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    DoMove(TileEntityConstants.Direction.DOWN);
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

                GameInfo gi = GameManager.Instance.MyGameBoard.GetGameInfo();
                if (gi.wasUpdated)
                {
                    // We own this player: send the others our data
                    string giString = gi.Stringify();
                    stream.SendNext(giString);
                    gi = new GameInfo();
                }
                else
                {
                    stream.SendNext("");
                }
            }
            else
            {
                // Network player, recieve data
                setUid((string)stream.ReceiveNext());

                string r = (string)stream.ReceiveNext();
                if (r != "")
                {
                    GameInfo gi = GameInfo.fromString(r);
                    GameManager.Instance.MyGameBoard.ProcessNewGameInfo(gi);
                }
            }
        }
    }
}
