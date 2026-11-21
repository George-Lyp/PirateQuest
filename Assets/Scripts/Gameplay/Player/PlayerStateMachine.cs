using UnityEngine;
namespace PirateQuest.Gameplay.Player
{
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Attack,
        Throw,
        Dash,
        Hurt,
        Dead
    }
    public sealed class PlayerStateMachine
    {
        public PlayerState Current { get; private set; } = PlayerState.Idle;
        public PlayerState Previous { get; private set; } = PlayerState.Idle;
        public float StateTime { get; private set; }
        private const float RunThreshold = 0.1f;
        private const float JumpThreshold = 0.01f;
        public void Tick(float deltaTime, bool isGrounded, float horizontalSpeed, float verticalSpeed)
        {
            StateTime += deltaTime;
            if (Current == PlayerState.Attack || Current == PlayerState.Throw || Current == PlayerState.Dash || Current == PlayerState.Hurt || Current == PlayerState.Dead)
            {
                return;
            }
            if (!isGrounded)
            {
                TryChange(verticalSpeed > JumpThreshold ? PlayerState.Jump : PlayerState.Fall);
            }
            else
            {
                TryChange(Mathf.Abs(horizontalSpeed) > RunThreshold ? PlayerState.Run : PlayerState.Idle);
            }
        }
        public bool TryChange(PlayerState next)
        {
            if (next == Current)
            {
                return false;
            }
            Previous = Current;
            Current = next;
            StateTime = 0;
            return true;
        }
    }
}
