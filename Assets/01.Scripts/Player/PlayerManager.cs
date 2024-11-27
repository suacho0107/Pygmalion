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
        // 플레이어 생성
        Vector3 spawnPosition = playerPos.isChecked ? playerPos.nextPosition : playerPos.currentPosition;
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        playerPos.isChecked = false;

        playerPos.currentPosition = spawnPosition;

        // 시네머신 CAM에 Follow에 로드된 플레이어 저장
        virtualCamera.Follow = player.transform;
    }
}
