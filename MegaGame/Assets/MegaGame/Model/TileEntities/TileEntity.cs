using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class TileEntity : MonoBehaviour
    {

        protected GameBoard gameBoard;
        private string uid;

        [SerializeField]
        protected float maxHealth = 0.0f;

        public override bool Equals(object other)
        {
            return ((TileEntity)other).uid.Equals(this.uid);
        }

        public override int GetHashCode()
        {
            long hashedValue = 5381;
            for (int i = 0; i < uid.Length; i++)
            {
                hashedValue = ((hashedValue << 5) + hashedValue) + uid[i];
            }
            return (int)hashedValue;
        }

        public string getUid()
        {
            return uid;
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


    }
}