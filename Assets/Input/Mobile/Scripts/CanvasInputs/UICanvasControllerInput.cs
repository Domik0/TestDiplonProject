using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualDance(bool virtualDanceState)
        {
            starterAssetsInputs.DanceInput(Random.Range(1, 7));
        }

        public void VirtualPunch(bool virtualPunchState)
        {
            starterAssetsInputs.PunchInput(virtualPunchState);
        }

        public void VirtualThrow(bool virtualThrowState)
        {
            starterAssetsInputs.ThrowObjectInput(virtualThrowState);
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsClient)
            {
                starterAssetsInputs =
                    NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<StarterAssetsInputs>();
            }
        }
    }

}

