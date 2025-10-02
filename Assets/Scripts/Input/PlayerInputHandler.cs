using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerInputHandler : MonoBehaviour
{
    public InputManager InputManager
    {  get; private set; }

    public InputSystem_Actions.PlayerActions PlayerActionMap
    { get; private set; }

    public Vector2 MovementInput
    { get; private set; }

    public Vector2 RotationInput
    { get; private set; }

    private void Awake()
    {
        if (InputManager == null)
        {
            InputManager = FindAnyObjectByType<InputManager>();

            if (InputManager == null)
            {
                Debug.LogError("Unable to find the global input manager.");
            }
            else
            {
                PlayerActionMap = InputManager.InputActions.Player;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput = HandleMovement();

        RotationInput = HandleRotation();
    }

    public Vector2 HandleMovement()
    {
        Vector2 playerMovement = Vector2.zero;

        playerMovement = PlayerActionMap.Move.ReadValue<Vector2>();

        return playerMovement;
    }

    public Vector2 HandleRotation()
    {
        Vector2 rotationMovement = Vector2.zero;

        rotationMovement = PlayerActionMap.Look.ReadValue<Vector2>();

        return rotationMovement;
    }
}
