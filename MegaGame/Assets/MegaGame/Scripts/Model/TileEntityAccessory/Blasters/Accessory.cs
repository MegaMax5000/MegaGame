using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class Accessory
    {
        protected TileEntity parentEntity;

        public Accessory(TileEntity parent)
        {
            this.parentEntity = parent;
        }

        public TileEntity GetParentEntity()
        {
            return this.parentEntity;
        }

        // It is intended for different accessories to
        // override this
        public void DoAction() {

        }
    }
}