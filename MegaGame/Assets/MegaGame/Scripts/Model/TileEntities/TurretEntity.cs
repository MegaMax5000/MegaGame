using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{ 
    public class TurretEntity : TileEntity
    {
        TimedActionManager.ActionItem action = null;
        private class ShootAction : Action {
            Accessory accessory;
            public ShootAction(Accessory a) {
                this.accessory = a;
            }
            public override void DoAction() {
                accessory.DoAction();
            }
        }
        Accessory accessory;
        public TurretEntity(GameBoard gb, string name, int maxHealth) : base(gb, name, maxHealth, TileEntityType.Turret)
        {   
        }

        public void Init()
        {
            this.accessory = AccessoryFactory.createAccessory(AccessoryFactory.AccessoryType.Counting, this);
            StartShooting(2);
        }

        public void StartShooting(float interval) {

            if (action != null) {
                Debug.Log("[TurretEntity] Turret is already shooting");
                return;
            }

            action =
            TimedActionManager
            .GetInstance()
            .RegisterAction(
                new ShootAction(this.accessory), 
                this, 
                interval
            );
        }

        public void StopShooting() {

            if (action == null) {
                return;
            }

            TimedActionManager
            .GetInstance()
            .UnregisterAction(action, this);

            action = null;
        }


        public override void DoTick() {

        }
    }
}