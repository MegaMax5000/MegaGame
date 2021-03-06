﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MegaGame
{
    public abstract class TileEntity : MonoBehaviourPunCallbacks, IPunObservable
    {

        protected GameBoard gameBoard;
        [SerializeField]
        protected string uid;
        
        [SerializeField]
        protected int maxHealth = 10;

        [SerializeField]
        public int Health { get; protected set; }

        public TextMeshPro HealthText;

        //public override bool Equals(object other)
        //{
        //    return ((TileEntity)other).uid.Equals(this.uid);
        //}

        //public override int GetHashCode()
        //{
        //    long hashedValue = 5381;
        //    for (int i = 0; i < uid.Length; i++)
        //    {
        //        hashedValue = ((hashedValue << 5) + hashedValue) + uid[i];
        //    }
        //    return (int)hashedValue;
        //}

        public enum TileEntityType
        {
            Player = 0,
            Damage = 1,
            Wall = 2, //temp, not sure if this will exist
            Turret = 3
        }

        public TileEntityType MyTileEntityType { get; protected set; }

        public string GetUid()
        {
            return uid;
        }

        public void SetUid(string newUid)
        {
            uid = newUid;
        }

        public GameBoard GetBoard()
        {
            return this.gameBoard;
        }

        public void SetBoard(GameBoard gb)
        {
            this.gameBoard = gb;
        }

        public TileEntity(GameBoard gb, string name, int maxHealth, TileEntityType tileEntityType)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.gameBoard = gb;
            this.uid = System.Guid.NewGuid().ToString();
            this.MyTileEntityType = tileEntityType;

            // Update health every .5 seconds
            TimedActionManager.GetInstance().RegisterAction(
                () =>
                {
                    DoTick();
                }
                , this, .5f);
        }

        // Start is called before the first frame update
        void Start()
        {
            this.maxHealth = 10;
            this.Health = this.maxHealth;
        }

        public virtual void DoTick()
        {
            HealthText.text = Health + "";
        }

        public float GetMaxHealth()
        {
            return this.maxHealth;
        }

        public void SetMaxHealth(int health)
        {
            this.maxHealth = (health >= 0 ? health : 0);
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return this.name;
        }


        /***
         * This will subtract the passed in amount from this.health. If this makes this.health
         * go below 0, we reset this.health to 0 and return amount of damage that was actually
         * applied.
         ***/
        public int TakeDamage(int amount)
        {
            Debug.Log(this.GetName() + " health before being shot for amount " + amount + " is " + this.Health);
            this.Health -= amount;
            if (this.Health < 0)
            {
                int surplusDamage = this.Health * -1;
                this.Health = 0;
                return amount - surplusDamage;
            }

            return amount;
        }


        public void ClaimTile(Tile tile) {
            tile.SetSide(GetTile().GetSide());
        }

        protected Tile GetTile() {
            Vector2Int pos = gameBoard.GetTileEntityPosition(this);
            Tile entityTile = gameBoard.GetTile(pos.x, pos.y);
            return entityTile;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            BlasterBullet b = other.gameObject.GetComponent<BlasterBullet>();
            if (b != null)
            {
                GameManager.Instance.MyGameBoard.DoDamageToTileEntity(this, b.Damage);
                //TakeDamage(b.Damage);
                //PhotonNetwork.Destroy(other.gameObject.GetComponent<PhotonView>());
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