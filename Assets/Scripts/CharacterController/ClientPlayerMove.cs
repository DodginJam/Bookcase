using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;

public class ClientPlayerMove : NetworkBehaviour
{
    [field: SerializeField]
    public PlayerInputHandler InputHandler
    {  get; private set; }

    [field: SerializeField]
    public PlayerController PlayerController
    { get; private set; }

    [field: SerializeField]
    public Transform PredictedVisual
    { get; private set; }

    void Awake()
    {
        InputHandler.enabled = false;
        PlayerController.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Read input if the device is the owner only.
        if (IsOwner)
        {
            InputHandler.enabled = true;
            PlayerController.SpawnCameraForPlayer();
        }

        // Only allow the player controller to be enabled on the server.
        if (IsServer)
        {
            PlayerController.enabled = true;
        }
    }

    /// <summary>
    /// The input is sent from the owner to the server.
    /// </summary>
    /// <param name="movementInput"></param>
    /// <param name="rotationInput"></param>
    [Rpc(target: SendTo.Server)]
    private void UpdateInputServerRpc(Vector2 movementInput, Vector2 rotationInput)
    {
        InputHandler.MovementInput = movementInput;
        InputHandler.RotationInput = rotationInput;
    }

    private void LateUpdate()
    {
        if (IsOwner && !IsServer)
        {
            // Character Controller input movement prediction.
            Vector3 predictedCharacterMovementVelocity = Vector3.zero;
            Vector3 globalMovement = new Vector3(InputHandler.MovementInput.x, 0, InputHandler.MovementInput.y);
            predictedCharacterMovementVelocity = PlayerController.MovementSpeed * Time.deltaTime * transform.TransformDirection(globalMovement);
            PredictedVisual.transform.position += predictedCharacterMovementVelocity;

            // Character Controller extenral forces / gravity movement prediction.
            // Non-implementation yet.

            // Character Transform Rotation Prediction.
            if (InputHandler.RotationInput.x != 0)
            {
                // The player object should rotate on only the Y axis to allow change in X and Z movement direction.
                Vector3 globalRotation = new Vector3(0, InputHandler.RotationInput.x, 0);

                PredictedVisual.transform.Rotate(PlayerController.RotationSpeed * Time.deltaTime * globalRotation);
            }

            Debug.Log("Local movement");
        }

        if (IsOwner)
        {
            // If not the owner (i.e. the gameobject for the player), then send updates of the player input to the server.
            UpdateInputServerRpc(InputHandler.MovementInput, InputHandler.RotationInput);
        }
    }
}
