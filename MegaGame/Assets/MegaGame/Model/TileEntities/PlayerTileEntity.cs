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

        public override void DoMove(Direction direction)
        {
            gameBoard.Move(this, direction);
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
            if (!cooldown)
            {
                if (Input.GetKeyDown("w"))
                {
                    DoMove(Direction.UP);
                }
                else if (Input.GetKeyDown("d"))
                {
                    DoMove(Direction.RIGHT);
                }
                else if (Input.GetKeyDown("a"))
                {
                    DoMove(Direction.LEFT);
                }
                else if (Input.GetKeyDown("s"))
                {
                    DoMove(Direction.DOWN);
                }
                Invoke("ResetCooldown", COOLDOWN_TIME);
                cooldown = true;
            }
        }


    }
}
