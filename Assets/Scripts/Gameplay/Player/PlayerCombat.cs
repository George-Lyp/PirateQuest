using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PirateQuest.Gameplay.Player
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        private const float RunThreshold = 0.1f;
        [Header("动画")]
        [SerializeField] private string[] attackStateNames = { "Player_Attack1", "Player_Attack2", "Player_Attack3" };
        [SerializeField] private string idleStateName = "Player_Idle(ws)";
        [SerializeField] private string runStateName = "Player_Run(ws)";
        [SerializeField] private float stateBlendTime = 0.05f;
        [Header("组件引用")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerController playerController;
        [Header("碰撞箱")]
        [SerializeField] private PlayerAttackHitbox[] hitboxes;
        private int comboIndex;
        private bool attackBuffered;
        public bool IsAttacking => comboIndex > 0;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            playerController = GetComponent<PlayerController>();
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            if (!playerController.IsGrounded)
            {
                return;
            }
            attackBuffered = true;
            if (comboIndex == 0)
            {
                StartAttack(1);
            }
        }
        private void StartAttack(int index)
        {
            comboIndex = index;
            attackBuffered = false;
            CloseAllHitboxes();
            playerController.StateMachine.TryChange(PlayerState.Attack);
            animator.Play(attackStateNames[index - 1], 0, 0f);
        }
        private void OnHitboxOpen(int segment)
        {
            CloseAllHitboxes();
            int index = segment - 1;
            if (index < 0 || index >= hitboxes.Length)
            {
                return;
            }
            hitboxes[index].Open();
        }
        private void OnHitBoxClose()
        {
            CloseAllHitboxes();
        }
        private void OnAttackEnd()
        {
            CloseAllHitboxes();
            if (attackBuffered && comboIndex < attackStateNames.Length)
            {
                StartAttack(comboIndex + 1);
                return;
            }
            EndAttack();
        }
        private void CloseAllHitboxes()
        {
            foreach (PlayerAttackHitbox hitbox in hitboxes)
            {
                hitbox.Close();
            }
        }
        private void EndAttack()
        {
            comboIndex = 0;
            attackBuffered = false;
            playerController.StateMachine.TryChange(PlayerState.Idle);
            bool isMoving = Mathf.Abs(rb.velocity.x) > RunThreshold;
            string nextState = (isMoving && playerController.IsGrounded) ? runStateName : idleStateName;
            animator.CrossFade(nextState, stateBlendTime);
        }
    }
}
