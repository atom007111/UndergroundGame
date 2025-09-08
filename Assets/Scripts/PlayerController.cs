using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    private Rigidbody rb;
    private Camera playerCamera;

    [Header("Jump & Crouch")]
    public float jumpForce = 5f;    // public, adjustable in Inspector
    public float crouchHeight = 1f;  // height of capsule when crouching
    public float standHeight = 2f;   // normal height
    public float crouchSpeed = 2.5f; // movement speed while crouching

    private bool isGrounded;
    private bool isCrouching = false;
    private CapsuleCollider capsuleCollider;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked; // Hide & lock cursor

        capsuleCollider = GetComponent<CapsuleCollider>();

    }

    void Update()
    {
        //jump
        CheckGround();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        //crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                capsuleCollider.height = crouchHeight;
            }
            else
            {
                capsuleCollider.height = standHeight;
            }
        }
        Look();
        Move();

    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float speed = moveSpeed;
        if (isCrouching) speed = crouchSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching) speed *= sprintMultiplier;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move * speed * Time.deltaTime);
    }

    void CheckGround()
    {
        // Cast a ray down to see if player is on the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

}
