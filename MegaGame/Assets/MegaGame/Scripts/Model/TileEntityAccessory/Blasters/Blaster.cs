﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public class Blaster : Accessory
    {
        protected int baseDamage = 1;
        protected int shootRange = 12;

        public Blaster(TileEntity parent) : base(parent) {
            this.parentEntity = parent;
        }

        public int GetBaseDamage()
        {
            return baseDamage;
        }

        public override void DoAction()
        {
            DoShoot();
        }

        // returns the entity which was shot (null if nothing was shot)
        public bool DoShoot()
        {
            GameBoard board = GameManager.Instance.MyGameBoard;
            Vector2Int entityPosition = board.GetTileEntityPosition(this.parentEntity);
            Tile entityTile = board.GetTile(this.parentEntity);
            // If on left side, shoot right. Otherwise if on right side, shoot left
            Vector3 bulletDirection = Tile.SIDE.LEFT.Equals(entityTile.GetSide()) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            Vector3 bulletSpawnPosition = this.parentEntity.transform.position + bulletDirection.normalized * 2;

            GameManager.Instance.SpawnStupidPhotonProjectile("BlasterBullet", bulletSpawnPosition, Quaternion.identity, bulletDirection, 50f, 1);
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