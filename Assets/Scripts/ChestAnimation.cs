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
        private Animator _animator;
        private Item _itemInChest;
        private NetworkVariable<bool> _gaveItemPlayerFlag = new NetworkVariable<bool>();
        private Storage _storage;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _storage = Storage.GetStorage();
            var rndItem = Random.Range(0, _storage.AllItems.Count);
            _itemInChest = _storage.AllItems[rndItem];
        }

        public void ChestOpen(InventoryWindow targetInventoryWindow)
        {
            if (!_gaveItemPlayerFlag.Value)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("ChestOpen", true);
                }
                _animator.SetBool("ChestOpen", true);
                targetInventoryWindow.targetInventory.AddItem(_storage.GetItem(_itemInChest.title));
                ChangeGiveServerRpc(true);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeGiveServerRpc(bool status)
        {
            _gaveItemPlayerFlag.Value = status;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateAnimatorServerRpc(string parametr, bool status)
        {
            _animator.SetBool(parametr, status);
        }
    }
}
