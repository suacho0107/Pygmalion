using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Select
{
    [Tooltip("��� ����")]
    public string[] contexts;

    [Tooltip("�ű����")]
    public string[] moveNum;
}

[System.Serializable]
public class SelectEvent
{
    public string name;

    public Vector2 line;
    public Select[] selects;
}
