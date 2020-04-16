using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{ 
    public class TurretEntity :  MoveableAIEntity
    {
        TimedActionManager.ActionItem action = null;

        public float FireRate = 5f;
        Accessory accessory;
        public TurretEntity(GameBoard gb, string name, int maxHealth) : base(gb, name, maxHealth, TileEntityType.Turret)
        {   
        }

        public void Initialize()
        {
            this.accessory = AccessoryFactory.createAccessory(AccessoryFactory.AccessoryType.Blaster, this);
            StartShooting(FireRate);
            StartAI();
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
               ()=>
               {
                   DoShoot();
               }, 
                this, 
                interval
            );
        }


        private void DoShoot()
        {
            this.accessory.DoAction();
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
    }
}