using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class ShockWaveEntity : DamageTileEntity
    {
        public static float SHOCKWAVE_SPEED = 300;
        private class ShockWaveAction : Action
        {
            public override void DoAction()
            {
            }
        }

        public ShockWaveEntity(GameBoard gb, string name, int maxHealth) : base(gb, name, maxHealth)
        {
            TimedAction action = new TimedAction(new ShockWaveAction(), SHOCKWAVE_SPEED);
            TimedActionManager.GetInstance().RegisterAction(action, this);
        }

        public override void DoTick()
        {
        }
    }
}