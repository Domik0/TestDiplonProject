using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ChestAnimation : NetworkBehaviour
    {
        private bool _chestZone;

        private int _animIDChestOpen;
        private int _animIDChestClose;

        private Animator _animator;
        private GameObject _prefabChest;
        private Item _itemInChest;
        private NetworkVariable<bool> _gaveItemPlayerFlag = new NetworkVariable<bool>();
        private Storage _storage;
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
            _storage = Storage.GetStorage();
            var rndItem = Random.Range(0, _storage.AllItems.Count);
            _itemInChest = _storage.AllItems[rndItem];
        }

        private void Update()
        {
            if (IsClient && IsOwner)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
        }

        public void ChestOpen(InventoryWindow targetInventoryWindow)
        {
            if(!_gaveItemPlayerFlag.Value && targetInventoryWindow.targetInventory.inventoryItems.Count < 3)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("ChestOpen", true);
                }
                _animator.SetBool("ChestOpen",true);
                targetInventoryWindow.targetInventory.AddItem(_storage.GetItem(_itemInChest.title));
                _gaveItemPlayerFlag.Value = true;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateAnimatorServerRpc(string parametr, bool status)
        {
            _animator.SetBool(parametr, status);
        }
    }
}
