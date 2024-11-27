using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        // �÷��̾� ����
        Vector3 spawnPosition = playerPos.isChecked ? playerPos.nextPosition : playerPos.currentPosition;
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        playerPos.isChecked = false;

        playerPos.currentPosition = spawnPosition;

        // �ó׸ӽ� CAM�� Follow�� �ε�� �÷��̾� ����
        virtualCamera.Follow = player.transform;
    }
}
