using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class CountAccessory : Accessory
    {
        protected TileEntity parentEntity;
        private int i = 0;
        public CountAccessory(TileEntity parent) : base(parent)
        {}

        public TileEntity GetParentEntity()
        {
            return this.parentEntity;
        }

        // It is intended for different accessories to
        // override this
        public override void DoAction() {
            Debug.Log(Time.time);
        }
    }
}