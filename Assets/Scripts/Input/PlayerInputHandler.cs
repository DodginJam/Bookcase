using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    public event Action InteractionTap;

    public event Action InteractionHold;

    public event Action AttackPress;

    public event Action AttackRelease;


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

    private void OnEnable()
    {
        EnableInputListeners();
    }

    private void OnDisable()
    {
        DisableInputListeners();
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
        Vector2 playerMovement = PlayerActionMap.Move.ReadValue<Vector2>();

        return playerMovement;
    }

    public Vector2 HandleRotation()
    {
        Vector2 rotationMovement = PlayerActionMap.Look.ReadValue<Vector2>();

        return rotationMovement;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            InteractionHold?.Invoke();
        }
        else if (context.interaction is TapInteraction)
        {
            InteractionTap?.Invoke();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttackPress?.Invoke();
        }
        else if (context.canceled)
        {
            AttackRelease?.Invoke();
        }
    }

    public void EnableInputListeners()
    {
        PlayerActionMap.Interact.performed += OnInteract;

        PlayerActionMap.Attack.started += OnAttack;

        PlayerActionMap.Attack.canceled += OnAttack;
    }

    public void DisableInputListeners()
    {
        PlayerActionMap.Interact.performed -= OnInteract;

        PlayerActionMap.Attack.started -= OnAttack;

        PlayerActionMap.Attack.canceled -= OnAttack;
    }
}
