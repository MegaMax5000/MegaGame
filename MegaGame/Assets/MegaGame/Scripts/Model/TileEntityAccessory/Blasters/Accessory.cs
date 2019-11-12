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
    }
}