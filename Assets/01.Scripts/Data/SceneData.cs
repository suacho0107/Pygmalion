using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneInfo
{
    public string sceneName;
    public int visitCount;
}

[CreateAssetMenu(fileName ="SceneData", menuName = "ScriptableObjects/SceneData", order = 1)]
public class SceneData : ScriptableObject
{
    public SceneInfo[] scenes;
}
