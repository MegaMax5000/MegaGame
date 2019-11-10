using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameInfo
{
    public byte Id { get; set; }

    public static byte[] Serialize(object o)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new System.IO.MemoryStream())
        {
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
    }

    public static object Deserialize(byte[] arrBytes)
    {
        using (var memStream = new System.IO.MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
    public Dictionary<string, EntityInfo> entityInfos = new Dictionary<string, EntityInfo>();
}
