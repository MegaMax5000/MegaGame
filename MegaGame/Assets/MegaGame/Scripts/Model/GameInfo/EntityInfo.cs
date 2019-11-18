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
        public Vector2Int position;
        public string uid;

        int health;

        protected override void RegisterFieldsToSerialize()
        {
            beginRegistration();
            register(uid);
            register(position.x);
            register(position.y);
        }

        // s will be in the form of (uid,x,y)
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
            ei.position.x = Int32.Parse(args[1]);
            ei.position.y = Int32.Parse(args[2]);
            return ei;
        }

        public EntityInfo(string uid)
        {
            this.uid = uid;
            this.openingDelim = SerializationConstants.ENTITY_INFO_START;
            this.closingDelim = SerializationConstants.ENTITY_INFO_END;
            this.delim = SerializationConstants.ENTITY_DELIM;
        }
    }
}