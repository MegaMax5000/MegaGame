﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Blaster : Accessory
    {
        protected int baseDamage = 1;
        protected int shootRange = 12;

        public Blaster(TileEntity parent) : base(parent) { }

        public int GetBaseDamage()
        {
            return baseDamage;
        }

        // returns the entity which was shot (null if nothing was shot)
        public bool DoShoot()
        {
            GameBoard board = this.parentEntity.GetBoard();
            Vector2Int entityPosition = board.GetTileEntityPosition(this.parentEntity);
            Tile entityTile = board.GetTile(this.parentEntity);
            // If on left side, shoot right. Otherwise if on right side, shoot left
            Vector2Int direction = Tile.SIDE.LEFT.Equals(entityTile.GetSide()) ? TileEntityConstants.DirectionVectors.RIGHT : TileEntityConstants.DirectionVectors.LEFT;
            Vector3 bulletDirection = new Vector3(direction.x, 1, direction.y);

            GameManager.Instance.SpawnStupidPhotonProjectile("BlasterBullet", this.parentEntity.transform.position, this.parentEntity.transform.rotation, bulletDirection, 10f, 1);
            //Vector2Int pos = entityPosition + direction;
            //for (int i = 0; i < this.shootRange && board.IsOnBoard(pos); ++i)
            //{
            //    Tile newTile = board.GetTile(pos);
                
            //    if (!newTile.GetSide().Equals(entityTile.GetSide()) && 
            //        newTile.GetEntities() != null && newTile.GetEntities().Count > 0)
            //    {
            //        // Tell GameBoard to damage everything in square
            //        foreach (TileEntity e in newTile.GetEntities())
            //        {
            //            board.DoDamageToTileEntity(e, this.baseDamage);
            //        }
            //        return true;
            //    }
            //    pos += direction;
            //}
            Debug.Log(this.parentEntity.GetName() + " did not shoot anything ");
            return false;
        }
    }
}