using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MegaGame
{
    public class EntityInfo : Info
    {
        public Vector2Int Position;
        public string Uid;
        public int Health;

        protected override void RegisterFieldsToSerialize()
        {
            beginRegistration();
            register(Uid);
            register(Health);
            register(Position.x);
            register(Position.y);
        }

        // s will be in the form of (uid,health,x,y)
        public static EntityInfo FromString(string s, char delim)
        {
            Debug.Log("Recieved entity info " + s);
            if (s.Length < 1)
            {
                return null;
            }

            string[] args = GetValues(s, delim);
            string id = args[0];
            EntityInfo ei = new EntityInfo(id);
            ei.Health = Int32.Parse(args[1]);
            ei.Position.x = Int32.Parse(args[2]);
            ei.Position.y = Int32.Parse(args[3]);
            return ei;
        }

        public EntityInfo(string uid)
        {
            this.Uid = uid;
            this.openingDelim = SerializationConstants.ENTITY_INFO_START;
            this.closingDelim = SerializationConstants.ENTITY_INFO_END;
            this.delim = SerializationConstants.ENTITY_DELIM;
        }
    }
}