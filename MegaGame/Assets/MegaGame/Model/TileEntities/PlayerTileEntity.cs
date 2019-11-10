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
            
        }

        public override void DoMove(Direction direction, EntityInfo info)
        {
            GameManager.Instance.MyGameBoard.Move(this, direction, info);
            cooldownTimer = Cooldown_time;
        }

        public void DoLurch(EntityInfo info)
        {
            GameManager.Instance.MyGameBoard.Move(this, Direction.LEFT, info);
            GameManager.Instance.MyGameBoard.Move(this, Direction.LEFT, info);
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
            if (cooldownTimer <= 0 && photonView.IsMine)
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
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    DoLurch(entityInfo);
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
