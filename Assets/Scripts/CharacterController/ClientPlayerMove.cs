using Unity.Netcode;
using UnityEngine;

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



    public const float UpdateFromServerToClientTickRate = 0.05f;

    public float UpdateFromServerToClientTimer
    { get; private set; } = 0;

    public const float UpdateFromClientToServerTickRate = 0.0166f;

    public float UpdateFromClientToServerTickTimer
    { get; private set; } = 0;

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
    private void UpdateInputToServerRpc(Vector2 movementInput, Vector2 rotationInput)
    {
        InputHandler.MovementInput = movementInput;
        InputHandler.RotationInput = rotationInput;
    }

    [Rpc(target: SendTo.NotAuthority)]
    private void UpdatePositionToClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;

        PredictedVisual.transform.localPosition = Vector3.zero;
        PredictedVisual.transform.localRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        // Predictive movement client side.
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

            // Debug.Log("Local movement");
        }

        // Send the client input to the server.
        if (IsOwner && IsClient)
        {
            UpdateFromClientToServerTickTimer += Time.deltaTime;

            if (UpdateFromClientToServerTickTimer >= UpdateFromClientToServerTickRate)
            {
                // If not the owner (i.e. the gameobject for the player), then send updates of the player input to the server.
                UpdateInputToServerRpc(InputHandler.MovementInput, InputHandler.RotationInput);

                UpdateFromClientToServerTickTimer = 0;
            }
        }

        // Recevie the server authority of the allowed movement.
        if (IsServer)
        {
            UpdateFromServerToClientTimer += Time.deltaTime;

            if (UpdateFromServerToClientTimer >= UpdateFromServerToClientTickRate)
            {
                UpdatePositionToClientRpc(PlayerController.transform.position, PlayerController.transform.rotation);

                UpdateFromServerToClientTimer = 0;
            }
        }
    }

    [Rpc(target: SendTo.Server)]
    public void SpawnPlayerCameraNetworkRpc()
    {
        // Spawn the camera on the server locally.
        GameObject playerCamera = Instantiate(PlayerController.CameraPrefab);



        // Assign the camera to mark the player camera as assigned.
        PlayerController.AssignedPlayerCamera = playerCamera;

        if (PlayerController.AssignedPlayerCamera.TryGetComponent<CameraController>(out CameraController cameraController))
        {
            // Reference check.
            if (PlayerController.PlayerCameraLead == null)
            {
                Debug.Log("PlayerCameraLead transform has not been assigned.");
                return;
            }

            // Assign the camera variables so it knows which player controller transform point to mimic.
            cameraController.InitialiseCameraController(PlayerController.PlayerCameraLead, PlayerController);
        }
        else
        {
            Debug.LogError("The gameobject spawned as the player camera does not have a camera controller assigned to it.");
        }



        NetworkObject playerCameraNetworkObject = playerCamera.GetComponent<NetworkObject>();
        playerCameraNetworkObject.SpawnWithOwnership(OwnerClientId);
    }
}
