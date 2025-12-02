using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(51)]
public class ClientCameraMove : NetworkBehaviour
{
    [field: SerializeField]
    public CameraController CameraControllerScript
    { get; private set; }

    public PlayerInputHandler PlayerInput
    { get; private set; }

    [field: SerializeField]
    public AudioListener AudioListener
    { get; private set; }

    public const float UpdateFromServerToClientTickRate = 0.05f;

    public float UpdateFromServerToClientTimer
    { get; private set; } = 0;

    public const float UpdateFromClientToServerTickRate = 0.0166f;

    public float UpdateFromClientToServerTickTimer
    { get; private set; } = 0;

    void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            // By default, disable the camera controller, the camera and the audio listners until whether the spawned camera is determined as owned by current client or a connected client.
            CameraControllerScript.enabled = false;
            CameraControllerScript.AttachedCamera.enabled = false;
            AudioListener.enabled = false;
        }
    }

    private void Start()
    {
        if (CameraControllerScript.PlayerControllerOwner != null)
        {
            PlayerInput = CameraControllerScript.PlayerControllerOwner.InputHandler;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // If the client owns this camera network behaviour, enable camera and listener - only works if the camera network behaviour has been assigned to the correct ownership upon spawn.
        if (IsOwner)
        {
            CameraControllerScript.AttachedCamera.enabled = true;
            AudioListener.enabled = true;

            Debug.Log("Network Owner Object Spawned");
        }
        else
        {
            Debug.Log("Network spawned object is not the owners");
        }

        // The camera controller is enabled only on server.
        if (IsServer)
        {
            CameraControllerScript.enabled = true;
        }
    }

    private void LateUpdate()
    {
        // Send the client input to the server.
        if (IsOwner && IsClient)
        {
            UpdateFromClientToServerTickTimer += Time.deltaTime;

            if (UpdateFromClientToServerTickTimer >= UpdateFromClientToServerTickRate)
            {
                UpdateInputToServerRpc();

                UpdateFromClientToServerTickTimer = 0;
            }
        }

        // Recevie the server authority of the allowed movement.
        if (IsServer)
        {
            UpdateFromServerToClientTimer += Time.deltaTime;

            if (UpdateFromServerToClientTimer >= UpdateFromServerToClientTickRate)
            {
                UpdatePositionToClientRpc(transform.position, transform.rotation);

                UpdateFromServerToClientTimer = 0;
            }
        }
    }

    [Rpc(target: SendTo.Server)]
    private void UpdateInputToServerRpc()
    {
        
    }

    [Rpc(target: SendTo.NotAuthority)]
    private void UpdatePositionToClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
