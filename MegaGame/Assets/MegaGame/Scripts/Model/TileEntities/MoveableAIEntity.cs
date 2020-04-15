using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MegaGame.TileEntity;

namespace MegaGame
{
    public class MoveableAIEntity : TileEntity
    {
        System.Random random = new System.Random();
        private static float SPEED = .4f;
        private static int numDirections = 4;
        private double[] directionWeights;
        private static class DIRS
        {
            public const int UP = 0;
            public const int DOWN = 1;
            public const int LEFT = 2;
            public const int RIGHT = 3;

        }

        public MoveableAIEntity(GameBoard gb, string name, int maxHealth, TileEntityType t) : base(gb, name, maxHealth, t)
        {
        }

        protected void StartAI()
        {
            random = new System.Random();
            directionWeights = new double[numDirections];
            AssignRandomProbability();

            TimedActionManager.GetInstance().RegisterAction(
                () =>
                {
                    ProbabilityUpdatePolicy();

                    int idx = 0;
                    if ( random.Next(0, 100) <= 79 )
                    {
                        // lower range
                        double upVal = directionWeights[DIRS.UP] * 100000;
                        // upper range
                        double downVal = upVal + directionWeights[DIRS.DOWN] * 100000;

                        // move up/down
                        if (random.Next(0, (int)downVal) <= upVal)
                        {
                            idx = DIRS.UP;
                        }
                        else
                        {
                            idx = DIRS.DOWN;
                        }
                    }
                    else
                    {
                        Debug.Log("else");
                        if (random.Next(0,2) == 0)
                        {
                            idx = DIRS.LEFT;
                        }
                        else
                        {
                            idx = DIRS.RIGHT;
                        }
                    }

                    switch (idx)
                    {
                        case DIRS.UP:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.UP);
                            break;
                        case DIRS.DOWN:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.DOWN);
                            break;
                        case DIRS.LEFT:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.LEFT);
                            break;
                        case DIRS.RIGHT:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.RIGHT);
                            break;
                        default:
                            break;
                    }
                }, this, SPEED
                );
        }

        private void AssignRandomProbability()
        {
            // Assign equal probabilities
            int sum = 0;
            for (int i = 0; i < numDirections; ++i)
            {
                int rand = random.Next(1, 1000);
                directionWeights[i] = rand;
                sum += rand;
            }

            for (int i = 0; i < numDirections; ++i)
            {
                directionWeights[i] /= sum;
            }
        }


        /***
         * Make it more likely to move in a direction which is
         * close to an opposing enemy, and less likely to move
         * near an ally. This is a turret, so moving up and down
         * will always have the same probability for now.
         * */
        protected void ProbabilityUpdatePolicy()
        {
            GameBoard gb = GameManager.Instance.MyGameBoard;
            Vector2Int myPosition = gb.GetTileEntityPosition(this);
            Vector2Int target = FindClosestFoe();

            if (target.Equals(myPosition))
            {
                // no preference
                AssignRandomProbability();
                return;
            }


            double t = (double)Math.Abs((target.x - myPosition.x))*(target.x - myPosition.x);
            directionWeights[DIRS.UP] = 1 - sigmoid(t);
            directionWeights[DIRS.DOWN] = sigmoid(t);

            directionWeights[DIRS.LEFT] = .5;
            directionWeights[DIRS.RIGHT] = .5;

            // normalize
            double sum = 0;
            for (int i = 0; i < directionWeights.Length; ++i)
            {
                sum += directionWeights[i];
            }

            for (int i = 0; i < directionWeights.Length; ++i)
            {
                directionWeights[i] /= sum;
            }

            string f = "[";
            for (int i = 0; i < 4; ++i)
            {
                f += directionWeights[i] + ",";
            }
            Debug.Log(f + "]");
        }

        private Vector2Int FindClosestFoe()
        {
            GameBoard gb = GameManager.Instance.MyGameBoard;
            Vector2Int myPosition = gb.GetTileEntityPosition(this);
            Tile myTile = gb.GetTile(this);

            TileEntity[] tileEntities = GameObject.FindObjectsOfType<TileEntity>();
            float closest = float.MaxValue;

            // Default to moving around where you are
            Vector2Int ret = myPosition;

            for (int i = 0; i < tileEntities.Length; ++i)
            {
                Vector2Int pos = gb.GetTileEntityPosition(tileEntities[i]);
                float dist = Vector2Int.Distance(myPosition, pos);
                
                if (dist < closest && myTile.GetSide() != gb.GetTile(tileEntities[i]).GetSide())
                {
                    ret = pos;
                }
            }

            return ret;
        }


        // map some value to the range 0,1
        private double sigmoid(double t)
        {
            return 1 / (1 + Math.Exp(-1 * t)); 
        }
    }
}
