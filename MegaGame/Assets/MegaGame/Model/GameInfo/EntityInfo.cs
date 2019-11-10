using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInfo
{
    public Vector2Int position;

    public string uid;

    int health;

    public EntityInfo(string uid)
    {
        this.uid = uid;
    }
}
