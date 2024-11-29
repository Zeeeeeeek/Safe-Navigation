using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    private Rigidbody2D _rb;
    private Vector2 _movement;

    public bool CanMove { get; set; } = true;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanMove)
        {
            _movement = Vector2.zero;
            return;
        }

        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        _movement = new Vector2(moveHorizontal, moveVertical).normalized;
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _movement * moveSpeed;
        if (_movement == Vector2.zero) return;
        var targetAngle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg - 90f;
        var angle = Mathf.LerpAngle(_rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        _rb.rotation = angle;
    }
}