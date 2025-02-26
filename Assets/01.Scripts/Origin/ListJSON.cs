using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class ListJSON : MonoBehaviour
{
    [Serializable]
    private class JsonWrapper<T>
    {
        public List<T> datas;
    }

    public static void SaveList<T>(List<T> datas, string path)
    {
        JsonWrapper<T> wrapper = new JsonWrapper<T>();
        wrapper.datas = datas;
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Application.persistentDataPath + path, json);
    }

    public static List<T> LoadList<T>(string path)
    {
        if(!File.Exists(Application.persistentDataPath + path))
        {
            Debug.Log("파일이 존재하지 않아 빈 리스트 반환");
            return new List<T>();
        }

        string json = File.ReadAllText(Application.persistentDataPath + path);
        JsonWrapper<T> wrapper = JsonUtility.FromJson<JsonWrapper<T>>(json);
        return wrapper.datas;
    }
}
