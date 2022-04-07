using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Cinemachine;
using UnityEngine;

public class PlayerFollow : Singleton<PlayerFollow>
{

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FollowPlayer(Transform transform)
    {
        if (cinemachineVirtualCamera == null) return;
        Debug.Log("Камеру врубай");
        cinemachineVirtualCamera.Follow = transform;

    }
}
