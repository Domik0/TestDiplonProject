using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class ChestAnimation : NetworkBehaviour
    {
        private bool _chestZone;

        private int _animIDChestOpen;
        private int _animIDChestClose;

        private Animator _animator;
        private GameObject _prefabChest;
        private bool _hasAnimator;

        private void Start()
        {
            if (IsClient && IsOwner)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (IsClient && IsOwner)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
        }

        public void ChestOpen()
        {
            _animator.SetTrigger("ChestOpen");
            //_animator.SetTrigger(_animIDChestClose);
        }
    }
}
