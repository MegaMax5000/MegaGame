using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class DamageTileEntity : TileEntity
    {
        protected int baseDamage;
        public DamageTileEntity(GameBoard gb, string name, int maxHealth) : base(gb, name, maxHealth, TileEntityType.Damage)
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public float DealDamage(TileEntity te)
        {
            float damageDealt = this.baseDamage - te.TakeDamage(this.baseDamage);
            return damageDealt;
        }
    }
}