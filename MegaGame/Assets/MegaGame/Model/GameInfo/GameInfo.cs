using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameInfo
{

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
            gi.entityInfos.Add(info.uid, info);
        }
        return gi;
    }

    public Dictionary<string, EntityInfo> entityInfos = new Dictionary<string, EntityInfo>();
}
