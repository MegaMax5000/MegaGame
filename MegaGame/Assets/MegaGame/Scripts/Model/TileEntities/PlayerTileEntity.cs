using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : TileEntity, IMoveable, IPunObservable
    {
        public float Cooldown_time = 0.1f;

        private Blaster blaster;

        public Blaster GetBlaster()
        {
            return blaster;
        }

   
        //never getting called right now
        public PlayerTileEntity(GameBoard gb, string name, float maxHealth, bool isPlayer1) : base(gb, name, maxHealth)
        {
            
        }

        public void DoMove(Vector2Int direction)
        {
            GameManager.Instance.MyGameBoard.Move(this, direction);
            cooldownTimer = Cooldown_time;
        }

        public override void DoTick()
        {
            // Do nothing
        }

        void Start()
        {
            blaster = new Blaster(this);
            this.maxHealth = 10f;
            this.health = this.maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            HandleMove();
        }

        private float cooldownTimer;
        private void HandleMove()
        {
            if (cooldownTimer <= 0 && photonView.IsMine)
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
                stream.SendNext(health);
                Debug.Log(GetUid() + " sending health : " + health);

                GameInfo gi = GameManager.Instance.MyGameBoard.GetGameInfo();
                if (gi.wasUpdated)
                {
                    // We own this player: send the others our data
                    string giString = gi.ToString();
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
                SetUid((string)stream.ReceiveNext());
                health = (float)stream.ReceiveNext();

                string r = (string)stream.ReceiveNext();
                if (r != "")
                {
                    Debug.Log(r);
                    GameInfo gi = GameInfo.FromString(r);
                    GameManager.Instance.MyGameBoard.ProcessNewGameInfo(gi);
                }
            }
        }
    }
}
