using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    private PlayerControls playerControls;

    public bool isHidden = false;

    // Singleton pattern starts here
    public static InputManager Instance {
        get {
            return _instance;
        }
    }
       
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }

        playerControls = new PlayerControls();
        
    }
    // Singleton pattern ends here
    
    private void OnEnable() {
        playerControls.Enable();
    }
    
    private void OnDisable() {
        playerControls.Disable();
    }
    
    // For dependent functions, in Start() follow same syntax as HideCursor, and then use the public bool to set values in other script
    private void Start() {
        playerControls.Player.HideCursor.performed += _ => DetermineHide();
    }

    private void DetermineHide() {
        isHidden = !isHidden;
    }

    // Non-dependent functions (Won't have to press once to do stuff and press again to not do stuff)
    public Vector2 GetPlayerMovement() {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }
    
    public Vector2 GetMouseDelta() {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }
    
    public bool PlayerJumpedThisFrame() {
        return playerControls.Player.Jump.triggered;
    }
}