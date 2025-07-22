using UnityEngine;
using UnityEngine.EventSystems;

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

        character.Move(direction * Time.deltaTime);
    }

    // ✅ Public method called instantly from Button click
    public void JumpFromButton()
    {
        if (!character.isGrounded)
        {
            Debug.Log("[Jump] Ignored jump — not grounded.");
            return;
        }

        direction = Vector3.up * jumpForce;
        Debug.Log("[Jump] Jumped via UI button.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
