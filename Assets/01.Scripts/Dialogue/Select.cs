using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Select
{
    [Tooltip("대사 내용")]
    public string[] contexts;

    [Tooltip("옮길라인")]
    public string[] moveNum;
}

[System.Serializable]
public class SelectEvent
{
    public string name;

    public Vector2 line;
    public Select[] selects;
}
