using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : TileEntity, IMoveable, IPunObservable
    {
        public float Cooldown_time = 0.1f;
        public TextMeshPro HealthText;

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
            cooldownTimer = Cooldown_time;
        }

        public override void DoTick()
        {
            // Do nothing
        }

        void Start()
        {
            blaster = new Blaster(this);
            this.maxHealth = 10;
            this.Health = this.maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            HandleMove();
            HealthText.text = Health + "";
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
                GameInfo gi = GameManager.Instance.MyGameBoard.GetGameInfo();
                if (gi.WasUpdated)
                {
                    // We own this player: send the others our data
                    string giString = gi.ToString();
                    stream.SendNext(giString);
                    Debug.Log(giString);

                    // reset the current game info to record new information
                    GameManager.Instance.MyGameBoard.ResetGameInfo();
                }
                else
                {
                    stream.SendNext("");
                }
            }
            else
            {
                // Network player, recieve data
                string r = (string)stream.ReceiveNext();
                if (r != "")
                {
                    GameInfo gi = GameInfo.FromString(r);
                    GameManager.Instance.MyGameBoard.ProcessNewGameInfo(gi);
                }
            }
        }
    }
}
