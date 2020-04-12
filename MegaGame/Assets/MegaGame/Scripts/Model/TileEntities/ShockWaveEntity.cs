﻿using System.Collections;
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
            TimedActionManager.GetInstance().RegisterAction(new ShockWaveAction(), this, SHOCKWAVE_SPEED);
        }

        public override void DoTick()
        {
        }
    }
}