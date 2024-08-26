using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        // �÷��̾� ����
        Vector3 spawnPosition = playerPos.isChecked ? playerPos.nextPosition : playerPos.currentPosition;
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        playerPos.isChecked = false;
    }
}
