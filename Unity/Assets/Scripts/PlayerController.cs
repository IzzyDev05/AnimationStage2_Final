using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform cameraTransform;

    private void Start() {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    void Update() {
        CheckIfGrounded();
        Move();
        Jump();

        HideCursor(); // Useful for disabling cursor after cliking out. Make sure final build isn't developer build!
    }

    private void CheckIfGrounded() {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
    }

    private void Move() {
        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        move = cameraTransform.forward.normalized * move.z + cameraTransform.right.normalized * move.x;
        move.y = 0f;

        controller.Move(move * (Time.deltaTime * playerSpeed));
    }

    private void Jump() {
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HideCursor() {
        // Cursor hiding wasn't working so I'm doing it every frame now, good code practices get rekt
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        /*
        if (Debug.isDebugBuild) {
            if (inputManager.isHidden) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        */
    }
}