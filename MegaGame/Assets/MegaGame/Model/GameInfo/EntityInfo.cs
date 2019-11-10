using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EntityInfo
{
    public Vector2Int position;
    public string uid;

    int health;

    // Generates (uid,x,y)
    public string Stringify()
    {
        return "(" + uid + "," + position.x + "," + position.y + ")";
    }

    // s will be in the form of (uid,x,y)
    public static EntityInfo fromString(string s)
    {
        // strip off parenthesis 
        s = s.Substring(1, s.Length - 1);
        string[] strs = s.Split(',');
        string id = strs[0];
        EntityInfo ei = new EntityInfo(id);
        ei.position.x = Int32.Parse(strs[1]);
        ei.position.y = Int32.Parse(strs[2]);
        return ei;
    }

    public EntityInfo(string uid)
    {
        this.uid = uid;
    }
}
