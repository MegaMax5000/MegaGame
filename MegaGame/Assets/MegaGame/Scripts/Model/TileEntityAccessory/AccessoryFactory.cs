using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public static class AccessoryFactory
    {
        public enum AccessoryType {
            Counting,
            Blaster
        }


        public static Accessory createAccessory(AccessoryType type, TileEntity t) {
            Accessory a = null;
            switch (type) {
                case AccessoryType.Counting:
                    a = new CountAccessory(t);
                    break;
                case AccessoryType.Blaster:
                    a = new Blaster(t);
                    break;
            }

            return a;
        }
    }
}