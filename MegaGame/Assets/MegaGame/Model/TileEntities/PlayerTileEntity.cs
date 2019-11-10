using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class PlayerTileEntity : MoveableTileEntity
    {
        private bool cooldown = false;

        public static float COOLDOWN_TIME = 0.4f;
        public PlayerTileEntity(GameBoard gb, string name, float maxHealth) : base(gb, name, maxHealth)
        {

        }

        public override void DoMove(Direction direction, EntityInfo info)
        {
            gameBoard.Move(this, direction, info);
        }

        public override void DoTick()
        {
            // Do nothing
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void ResetCooldown()
        {
            cooldown = false;
        }

        // Update is called once per frame
        void Update()
        {
            EntityInfo entityInfo = new EntityInfo(this.getUid());
            if (!cooldown)
            {
                if (Input.GetKeyDown("w"))
                {
                    DoMove(Direction.UP, entityInfo);
                }
                else if (Input.GetKeyDown("d"))
                {
                    DoMove(Direction.RIGHT, entityInfo);
                }
                else if (Input.GetKeyDown("a"))
                {
                    DoMove(Direction.LEFT, entityInfo);
                }
                else if (Input.GetKeyDown("s"))
                {
                    DoMove(Direction.DOWN, entityInfo);
                }
                Invoke("ResetCooldown", COOLDOWN_TIME);
                cooldown = true;
            }
        }
    }
}
