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
    /// ����� ��� �������� �� ��������
    /// </summary>
    /// <param name="transform">������ �� ������� ����� ������� ������</param>
    public void FollowPlayer(Transform transform)
    {
        if (cinemachineVirtualCamera == null) return;
        cinemachineVirtualCamera.Follow = transform;

    }
}
