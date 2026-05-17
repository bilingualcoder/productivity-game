using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    public Sprite idleDown;
    public Sprite idleUp;
    public Sprite idleSide;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // PlayerInput이 자동으로 호출함
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        ChangeDirectionSprite();
    }

    private void ChangeDirectionSprite()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        // 세로 입력이 더 크면 위/아래
        if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
        {
            if (moveInput.y > 0)
            {
                spriteRenderer.sprite = idleUp;
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.sprite = idleDown;
                spriteRenderer.flipX = false;
            }
        }
        // 가로 입력이 더 크면 좌/우
        else
        {
            spriteRenderer.sprite = idleSide;

            if (moveInput.x < 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
    }
}