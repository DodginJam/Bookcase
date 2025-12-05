using System.Collections;
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

    public ulong CameraNetworkID
    { get; private set; } = 0;

    public ulong PlayerNetworkID
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

    /// <summary>
    /// Coroutine waits for the camera and player ID's to be received before updating the local player controller with references to the client side networked camera.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForCameraNetworkID()
    {
        Debug.LogWarning("Coroutine started");

        bool isIDsReceived = false;

        while (isIDsReceived == false)
        {
            if (CameraNetworkID != 0 && PlayerNetworkID != 0)
            {
                isIDsReceived = true;
            }
            else
            {
                yield return null;
            }
        }

        Debug.LogWarning("Coroutine ended");

        UpdateLocalPlayerController(CameraNetworkID, PlayerNetworkID);
    }

    [Rpc(target:SendTo.NotServer)]
    public void SendNetworkIDsToClientRpc(ulong cameraNetworkID, ulong playerControlerNetworkID)
    {
        Debug.Log("Network IDs sent");

        CameraNetworkID = cameraNetworkID;
        PlayerNetworkID = playerControlerNetworkID;

        Debug.Log($"CameraNetworkID: {CameraNetworkID}");
        Debug.Log($"PlayerNetworkID: {PlayerNetworkID}");
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

            if (!IsServer)
            {
                Debug.Log("Coroutine Started in IsClientCheck");
                StartCoroutine(WaitForCameraNetworkID());
            }
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

    /// <summary>
    /// Taking the network IDs or the player and the camera, assign to the local player controller the correct reference for the networked camera gameobject that the server has spawned.
    /// </summary>
    /// <param name="cameraNetworkObjectID"></param>
    /// <param name="playerNetworkObjectID"></param>
    public void UpdateLocalPlayerController(ulong cameraNetworkObjectID, ulong playerNetworkObjectID)
    {
        GameObject clientCamera = NetworkManager.Singleton.SpawnManager.SpawnedObjects[cameraNetworkObjectID].gameObject;

        GameObject clientPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectID].gameObject;
        PlayerController playerController = clientPlayer.GetComponent<PlayerController>();

        // Assign the camera to mark the player camera as assigned.
        playerController.AssignedPlayerCamera = clientCamera;

        if (playerController.AssignedPlayerCamera.TryGetComponent<CameraController>(out CameraController cameraController))
        {
            // Reference check.
            if (playerController.PlayerCameraLead == null)
            {
                Debug.Log("PlayerCameraLead transform has not been assigned.");
                return;
            }

            // Assign the camera variables so it knows which player controller transform point to mimic.
            cameraController.InitialiseCameraController(playerController.PlayerCameraLead, playerController);
        }
        else
        {
            Debug.LogError("The gameobject spawned as the player camera does not have a camera controller assigned to it.");
        }
    }
}
