using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
        public bool punch;
        public int dance;
        public bool throwObject;

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void DanceInput(int newDanceIdState)
        {
            dance = newDanceIdState;
        }

        public void PunchInput(bool punchState)
        {
            punch = punchState;
        }

        public void ThrowObjectInput(bool throwObjectState)
        {
            throwObject = throwObjectState;
        }
    }
}