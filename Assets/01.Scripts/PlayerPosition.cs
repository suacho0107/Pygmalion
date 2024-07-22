using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPosition", menuName = "ScriptableObjects/PlayerPosition", order = 1)]
public class PlayerPosition : ScriptableObject
{
    public Vector3 currentPosition;
    public Vector3 nextPosition;
    public bool isChecked;
}
