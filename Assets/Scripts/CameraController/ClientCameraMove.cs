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
        CameraControllerScript.enabled = false;
        CameraControllerScript.AttachedCamera.enabled = false;
        AudioListener.enabled = false;
    }

    private void Start()
    {
        PlayerInput = CameraControllerScript.PlayerControllerOwner.InputHandler;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient)
        {
            CameraControllerScript.AttachedCamera.enabled = true;
            AudioListener.enabled = true;

            Debug.Log("Network Owner Object Spawned");
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
                UpdatePositionToClientRpc();

                UpdateFromServerToClientTimer = 0;
            }
        }
    }

    [Rpc(target: SendTo.Server)]
    private void UpdateInputToServerRpc()
    {

    }

    [Rpc(target: SendTo.NotAuthority)]
    private void UpdatePositionToClientRpc()
    {

    }
}
