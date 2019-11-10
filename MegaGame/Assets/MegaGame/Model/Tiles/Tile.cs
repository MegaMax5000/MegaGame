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

        public GameBoard gameBoard;

        private Vector3 entityOffset = new Vector3(0, 2, 0);

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

        public bool RemoveTileEntity(string uid)
        {
            foreach (var te in tileEntities)
            {
                if (te.getUid() == uid) return tileEntities.Remove(te);
            }

            return false;
        }

        public void AddTileEntity(TileEntity te)
        {
            tileEntities.Add(te);
        }

        public void AddTileEntity(string uid)
        {
            foreach (var te in tileEntities)
            {
                if (te.getUid() == uid) tileEntities.Add(te);
            }

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

        public TileEntity GetEntity(string uid)
        {
            foreach (var te in tileEntities)
            {
                if (te.getUid() == uid) return te;
            }

            return null;
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
                if (te != null)
                {
                    te.transform.position = this.transform.position + entityOffset;
                    te.DoTick();
                }
            }
        }
    }
}