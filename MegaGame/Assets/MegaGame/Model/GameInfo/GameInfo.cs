using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MegaGame
{
    public class GameInfo
    {
        public Dictionary<string, EntityInfo> entityInfos = new Dictionary<string, EntityInfo>();
        public bool wasUpdated = false;

        // int the form of (v1,v2,v3),(v1,v2,v3),...
        public string Stringify()
        {
            string ret = "";
            foreach (EntityInfo v in entityInfos.Values)
            {
                ret += v.Stringify() + '$';
            }
            if (ret.EndsWith("$"))
            {
                ret = ret.Substring(0, ret.Length - 1);
            }
            return ret;
        }

        public static GameInfo fromString(string s)
        {
            GameInfo gi = new GameInfo();

            string[] strs = s.Split('$');

            for (int i = 0; i < strs.Length; ++i)
            {
                EntityInfo info = EntityInfo.fromString(strs[i]);

                if (info != null)
                {
                    gi.entityInfos.Add(info.uid, info);
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
            wasUpdated = true;
            return UpdateOrAddToEntityInfoDictionary(value.uid, value);
        }

        // Will return true if added a new entry, false if updated existing one
        public bool UpdateOrAddToEntityInfoDictionary(string key, EntityInfo value)
        {
            // Update the position
            if (entityInfos.ContainsKey(key))
            {
                entityInfos[key] = value;
                return false;
            }
            else
            {
                entityInfos.Add(key, value);
                return true;
            }
        }

        public EntityInfo GetEntityInfoOrDefault(string uid, EntityInfo def)
        {
            if (entityInfos.ContainsKey(uid))
            {
                EntityInfo inf;
                if (entityInfos.TryGetValue(uid, out inf))
                {
                    return inf;
                }

            }
            // Return default value
            return def;
        }
    }
}