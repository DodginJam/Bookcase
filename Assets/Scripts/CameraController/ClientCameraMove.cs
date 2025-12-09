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

    public float PredictivePitch
    { get; private set; }

    void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            // By default, disable the camera controller, the camera and the audio listners until whether the spawned camera is determined as owned by current client or a connected client.
            CameraControllerScript.enabled = false;
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

        AssignLocalCamera();
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

        if (IsOwner)
        {
            Debug.Log("Network Owner Object Spawned");

            if (!IsServer)
            {
                Debug.Log("Coroutine Started in IsClientCheck");
                StartCoroutine(WaitForCameraNetworkID());
            }
            else
            {
                AssignLocalCamera();
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
        // Client side prediction for camera.
        if (IsOwner && !IsServer)
        {
            transform.position = CameraControllerScript.NetworkTransformToFollow.position;
            transform.rotation = CameraControllerScript.NetworkTransformToFollow.rotation;

            PredictivePitch -= CameraControllerScript.PlayerControllerOwner.InputHandler.RotationInput.y * Time.deltaTime * CameraControllerScript.PlayerControllerOwner.RotationSpeed;

            PredictivePitch = Mathf.Clamp(PredictivePitch, -85, 85);

            CameraControllerScript.NetworkTransformToFollow.localRotation = Quaternion.Euler(PredictivePitch, CameraControllerScript.NetworkTransformToFollow.localRotation.y, CameraControllerScript.NetworkTransformToFollow.localRotation.z);
        }

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
                UpdatePositionAndPitchToClientRpc(transform.position, transform.rotation, CameraControllerScript.CameraPitch);

                UpdateFromServerToClientTimer = 0;
            }
        }
    }

    [Rpc(target: SendTo.Server)]
    private void UpdateInputToServerRpc()
    {
        
    }

    [Rpc(target: SendTo.NotAuthority)]
    private void UpdatePositionAndPitchToClientRpc(Vector3 position, Quaternion rotation, float pitch)
    {
        transform.position = position;
        transform.rotation = rotation;

        PredictivePitch = pitch;
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
            if (playerController.NetworkCameraLead == null)
            {
                Debug.Log("NetworkCameraLead transform has not been assigned.");
                return;
            }

            // Assign the camera variables so it knows which player controller transform point to mimic.
            cameraController.InitialiseCameraController(playerController.NetworkCameraLead, playerController.LocalCameraLead, playerController);
        }
        else
        {
            Debug.LogError("The gameobject spawned as the player camera does not have a camera controller assigned to it.");
        }
    }

    public void AssignLocalCamera()
    {
        if (IsOwner)
        {
            // Find the local scene camera
            if (Camera.main.gameObject.TryGetComponent<SceneCamera>(out SceneCamera sceneCamera))
            {
                sceneCamera.AssignedCameraController = CameraControllerScript;
            }
        }
        else
        {
            Debug.LogWarning("No Owner");
        }
    }
}
