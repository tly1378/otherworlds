using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Setting
{
    public static string itemFilePath = "\\items.json";

    public static void PutJson<T>(string relativePath, T _object) {
        string json = JsonUtility.ToJson(_object, true);
        string path = Application.dataPath + $"/{relativePath}";
        WriteToTxt(path, json);
    }

    public static T GetJson<T>(string relativePath) {
        return JsonUtility.FromJson<T>(ReadFromTxt(Application.dataPath + relativePath));
    }

    internal static void WriteToTxt(string path, string text) {
        using StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.Write(text);
    }

    internal static string ReadFromTxt(string path) {
        using StreamReader streamReader = new StreamReader(path);
        return streamReader.ReadToEnd();//物品数量增大后需异步化
    }
}
