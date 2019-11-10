using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MegaGame
{
    public abstract class Tile : MonoBehaviour
    {
        public enum SIDE
        {
            LEFT,
            RIGHT
        }

        protected SIDE side;

        [SerializeField]
        protected List<TileEntity> tileEntities = new List<TileEntity>();

        protected GameBoard gameBoard;

        public Tile(GameBoard gb, SIDE side)
        {
            this.gameBoard = gb;
            this.side = side;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public SIDE GetSide()
        {
            return side;
        }

        public bool RemoveTileEntity(TileEntity te)
        {
            return tileEntities.Remove(te);
        }

        public void AddTileEntity(TileEntity te)
        {
            tileEntities.Add(te);
        }

        public void SetSide(SIDE side)
        {
            this.side = side;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddEntity(TileEntity entity)
        {
            tileEntities.Add(entity);
        }

        public List<TileEntity> GetEntities()
        {
            return this.tileEntities;
        }

        public bool DeleteEntity(TileEntity entity)
        {
            return tileEntities.Remove(entity);
        }

        public abstract void ApplyEffect(TileEntity tileEntity);

        public virtual void DoTick()
        {
            foreach (TileEntity te in tileEntities)
            {
                te.DoTick();
            }
        }
    }
}