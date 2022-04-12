using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Cinemachine;
using UnityEngine;

public class PlayerFollow : Singleton<PlayerFollow>
{

    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// Метод для слежение за объектом
    /// </summary>
    /// <param name="transform">Объект за которым будет следить камера</param>
    public void FollowPlayer(Transform transform)
    {
        if (cinemachineVirtualCamera == null) return;
        cinemachineVirtualCamera.Follow = transform;

    }
}
