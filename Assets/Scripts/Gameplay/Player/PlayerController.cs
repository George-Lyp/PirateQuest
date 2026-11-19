using UnityEngine;
using UnityEngine.InputSystem;

namespace PirateQuest.Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("移动")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 12f;

        [Header("地面检测")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.12f;
        [SerializeField] private LayerMask groundLayer;

        [Header("调试")]
        [SerializeField] private bool drawGroundGizmo = true;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private readonly PlayerStateMachine stateMachine = new PlayerStateMachine();

        private float moveInput;
        private bool jumpRequested;
        private bool isGrounded;

        public bool IsGrounded => isGrounded;
        public PlayerStateMachine StateMachine => stateMachine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateAnimator();
            FaceMoveDirection();
        }

        private void FixedUpdate()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (jumpRequested)
            {
                jumpRequested = false;
                if (isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }

            stateMachine.Tick(Time.fixedDeltaTime, isGrounded, rb.velocity.x, rb.velocity.y);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
        }

        private void FaceMoveDirection()
        {
            if (Mathf.Abs(moveInput) < 0.01f)
            {
                return;
            }

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>().x;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            jumpRequested = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGroundGizmo)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
