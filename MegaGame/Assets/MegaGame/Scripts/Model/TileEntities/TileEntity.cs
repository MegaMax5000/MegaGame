﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class TileEntity : MonoBehaviourPunCallbacks
    {

        protected GameBoard gameBoard;
        [SerializeField]
        protected string uid;

        [SerializeField]
        protected float maxHealth = 10.0f;

        [SerializeField]
        protected float health = 10.0f;

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

        public TileEntity(GameBoard gb, string name, float maxHealth)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.gameBoard = gb;
            this.uid = System.Guid.NewGuid().ToString();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public abstract void DoTick();

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
        public float TakeDamage(float amount)
        {
            Debug.Log(this.GetName() + " health before being shot for amount " + amount + " is " + this.health);
            this.health -= amount;
            if (this.health < 0)
            {
                float surplusDamage = this.health * -1;
                this.health = 0;
                return amount - surplusDamage;
            }

            return amount;
        }

    }
}