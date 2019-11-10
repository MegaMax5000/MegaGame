using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class TileEntity : MonoBehaviour
    {
        protected GameObject gameObject;

        protected GameBoard gameBoard;

        [SerializeField]
        protected string name = null;
        [SerializeField]
        protected float maxHealth = 0.0f;


        public TileEntity(GameBoard gb, string name, float maxHealth)
        {
            this.name = name;
            this.maxHealth = maxHealth;
            this.gameBoard = gb;
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