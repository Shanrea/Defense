using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkspeed = 5f;
    public float runspeed = 8f;
    public float jumpImpulse = 8f;
    public float airWalkSpeed = 3f;
    public float fallMultiplier = 2.5f;

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        return IsRunning ? runspeed : walkspeed;
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    Rigidbody2D rb;
    Animator animator;
    Collider2D coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity && IsAlive)
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        if (IsAlive)
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = fallMultiplier / 2f;
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
        else
        {
            rb.gravityScale = 1f;
            rb.velocity = Vector2.zero; // Arrête le mouvement du joueur
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);

        if (!IsAlive)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        IsAlive = false;
        animator.SetTrigger("Death");
        coll.enabled = true; // Garde le collider activé pour empêcher le joueur de passer à travers la plateforme

        // Arrête le jeu après un délai pour permettre à l'animation de mort de jouer
        StartCoroutine(StopGameAfterDelay());
    }

    private IEnumerator StopGameAfterDelay()
    {
        // Attends la fin de l'animation de mort plus une seconde supplémentaire
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1.0f);
        Time.timeScale = 0;
    }
}
