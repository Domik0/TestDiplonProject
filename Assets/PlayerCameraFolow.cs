using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerCameraFolow: Singleton<PlayerCameraFolow>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    public void FollowPlayer(Transform transform)
    {
        // not all scenes have a cinemachine virtual camera so return in that's the case
        if (cinemachineVirtualCamera == null) return;
        cinemachineVirtualCamera.Follow = transform;
    }
}
