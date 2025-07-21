using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;

    [Header("Movement Settings")]
    public float jumpForce = 8f;
    public float gravity = 9.81f * 2f;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    private void Update()
    {
        direction += gravity * Time.deltaTime * Vector3.down;

        if (character.isGrounded)
        {
            direction = Vector3.down;

#if UNITY_EDITOR
            if (Input.GetButton("Jump"))
            {
                direction = Vector3.up * jumpForce;
                Debug.Log("[Input] Keyboard jump triggered.");
            }
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchPos = touch.position;

                    float screenWidth = Screen.width;
                    float screenHeight = Screen.height;

                    float horizontalBuffer = screenWidth * 0.05f;
                    float verticalBuffer = screenHeight * 0.05f;

                    if (touchPos.x > horizontalBuffer && touchPos.x < screenWidth - horizontalBuffer &&
                        touchPos.y > verticalBuffer && touchPos.y < screenHeight - verticalBuffer)
                    {
                        direction = Vector3.up * jumpForce;
                        Debug.Log($"[Input] Precise Touch jump at ({touchPos.x}, {touchPos.y})");
                    }
                    else
                    {
                        Debug.Log("[Input] Touch ignored: too close to screen edge.");
                    }
                }
            }
#endif
        }

        character.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
