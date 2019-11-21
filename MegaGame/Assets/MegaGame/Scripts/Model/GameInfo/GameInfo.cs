using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MegaGame
{
    public class GameInfo : Info
    {
        // k: uid, v: EntityInfo
        public Dictionary<string, EntityInfo> EntityInfos = new Dictionary<string, EntityInfo>();

        public bool WasUpdated = false;

        protected override void RegisterFieldsToSerialize()
        {
            beginRegistration();
            foreach (EntityInfo inf in EntityInfos.Values)
            {
                register(inf);
            }
        }

        public GameInfo()
        {
            this.openingDelim = SerializationConstants.GAME_INFO_START;
            this.closingDelim = SerializationConstants.GAME_INFO_END;
            this.delim = SerializationConstants.GAME_INFO_DELIM;

            WasUpdated = false;
        }

        public static GameInfo FromString(string s)
        {
            GameInfo gi = new GameInfo();

            string[] strs = GetValues(s, SerializationConstants.GAME_INFO_DELIM);

            for (int i = 0; i < strs.Length; ++i)
            {
                EntityInfo info = EntityInfo.FromString(strs[i], SerializationConstants.ENTITY_DELIM);

                if (info != null)
                {
                    gi.EntityInfos.Add(info.Uid, info);
                }
                else
                {
                    Debug.Log("[GameInfo.fromString] EntityInfo returned from fromString was null");
                }
            }
            return gi;
        }


        public bool UpdateOrAddToEntityInfoDictionary(EntityInfo value)
        {
            WasUpdated = true;
            return UpdateOrAddToEntityInfoDictionary(value.Uid, value);
        }

        // Will return true if added a new entry, false if updated existing one
        public bool UpdateOrAddToEntityInfoDictionary(string key, EntityInfo value)
        {
            // Update the position
            if (EntityInfos.ContainsKey(key))
            {
                EntityInfos[key] = value;
                return false;
            }
            else
            {
                EntityInfos.Add(key, value);
                return true;
            }
        }

        public EntityInfo GetEntityInfoOrDefault(string uid, EntityInfo def)
        {
            if (EntityInfos.ContainsKey(uid))
            {
                EntityInfo inf;
                if (EntityInfos.TryGetValue(uid, out inf))
                {
                    return inf;
                }

            }
            // Return default value
            return def;
        }
    }
}