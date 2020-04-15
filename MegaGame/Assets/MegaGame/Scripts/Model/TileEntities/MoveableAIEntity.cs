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
        private static System.Random random = new System.Random();
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

                    string s = "";
                    for (int i = 0; i < directionWeights.Length; ++i)
                    {
                        s += directionWeights[i] + " ";
                    }
                    double upVal = directionWeights[DIRS.UP] * 100000;
                    double downVal = directionWeights[DIRS.DOWN] * 100000;
                    double lVal = directionWeights[DIRS.LEFT] * 100000;
                    double rVal = directionWeights[DIRS.RIGHT] * 100000;

                    double lower = lVal + rVal;
                    double upper = lower + downVal + upVal;

                    int idx = 0;
                    if ( (int)upper <= 0 || random.Next(0, (int)upper) <= lower )
                    {
                        lVal += rVal;
                        // move up/down
                        if ((int)lVal <= 0 || random.Next(0, (int)lVal) <= rVal)
                        {
                            idx = DIRS.RIGHT;
                        }
                        else
                        {
                            idx = DIRS.LEFT;
                        }
                    }
                    else
                    {
                        downVal += upVal;
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

                    switch (idx)
                    {
                        case DIRS.UP:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.UP, false);
                            break;
                        case DIRS.DOWN:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.DOWN, false);
                            break;
                        case DIRS.LEFT:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.LEFT, false);
                            break;
                        case DIRS.RIGHT:
                            GameManager.Instance.MyGameBoard.DoMoveTileEntity(this, TileEntityConstants.DirectionVectors.RIGHT, false);
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
            Vector2Int ally = FindClosestAlly();

            if (target.Equals(myPosition))
            {
                // no preference
                AssignRandomProbability();
                return;
            }


            double upPenalty = sigmoid(Math.Abs((myPosition.x - target.x)) * (myPosition.x - target.x));
            double downPenalty = 1 - upPenalty;
            double leftPenalty = (upPenalty + downPenalty) / 2; ;//sigmoid(Math.Abs((myPosition.y - target.y)) * (myPosition.y - target.y));
            double rightPenalty = (upPenalty + downPenalty) / 2; ; //1 - leftPenalty;

            directionWeights[DIRS.UP] = upPenalty;
            directionWeights[DIRS.DOWN] = downPenalty;
            directionWeights[DIRS.LEFT] = leftPenalty;
            directionWeights[DIRS.RIGHT] = rightPenalty;

            // Update weights based on ally distance
            double downAward = sigmoid(Math.Abs((myPosition.x - ally.x)) * (myPosition.x - ally.x));
            double upAward = 1 - downAward;
            double rightAward = (downAward + upAward) / 2;//sigmoid(Math.Abs((myPosition.y - ally.y)) * (myPosition.y - ally.y));
            double leftAward = (downAward + upAward) / 2; //1 - rightAward;

            directionWeights[DIRS.UP] += upAward;
            directionWeights[DIRS.DOWN] += downAward;
            directionWeights[DIRS.LEFT] += leftAward;
            directionWeights[DIRS.RIGHT] += rightAward;
            
            /*if (!gb.IsOnBoard(TileEntityConstants.DirectionVectors.UP + myPosition) )
            {
                directionWeights[DIRS.UP] = 0;
            }
            if (!gb.IsOnBoard(TileEntityConstants.DirectionVectors.DOWN + myPosition))
            {
                directionWeights[DIRS.DOWN] = 0;
            }
            if (!gb.IsOnBoard(TileEntityConstants.DirectionVectors.LEFT + myPosition))
            {
                directionWeights[DIRS.LEFT] = 0;
            }
            if (!gb.IsOnBoard(TileEntityConstants.DirectionVectors.RIGHT + myPosition))
            {
                directionWeights[DIRS.RIGHT] = 0;
            }*/
            normWeights();
        }

        private Vector2Int FindClosestFoe()
        {
            return FindClosestOnSide(false);
        }

        private Vector2Int FindClosestAlly()
        {
            return FindClosestOnSide(true);
        }

        private Vector2Int FindClosestOnSide(bool ally)
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
                
                if (tileEntities[i] == this)
                {
                    continue;
                }
                if (dist < closest)
                {
                    if (
                        (ally && myTile.GetSide() == gb.GetTile(tileEntities[i]).GetSide())
                        || (!ally && myTile.GetSide() != gb.GetTile(tileEntities[i]).GetSide())
                        )
                    {
                        ret = pos;
                    }
                }
            }

            return ret;
        }



        // map some value to the range 0,1
        private double sigmoid(double t)
        {
            return 1 / (1 + Math.Exp(-1 * t)); 
        }

        private void normWeights()
        {
            double s = directionWeights[0] + directionWeights[1] + directionWeights[2] + directionWeights[3];
            directionWeights[DIRS.UP] /= s;
            directionWeights[DIRS.DOWN] /= s;
            directionWeights[DIRS.LEFT] /= s;
            directionWeights[DIRS.RIGHT] /= s;

        }
    }
}
