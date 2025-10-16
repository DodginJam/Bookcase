using UnityEngine;

[DefaultExecutionOrder(50)]
public class CameraController : MonoBehaviour
{
    public Camera PlayerCamera
    {  get; private set; }

    public Transform FirstPersonCameraHolder
    { get; private set; }

    /// <summary>
    /// Keeps tracks of the current pitch angle of the gameobject the camera is to match the rotation of.
    /// </summary>
    public float CameraPitch
    { get; private set; }

    /// <summary>
    /// Whether the camera is a first person or third person camera.
    /// </summary>
    public CameraPositionState CameraPosition
    { get; private set; }

    /// <summary>
    /// Reference to the player controller and the player input system contained within.
    /// </summary>
    public PlayerController PlayerController
    { get; private set; }

    private void Awake()
    {
        if (PlayerCamera == null)
        {
            PlayerCamera = Camera.main;

            if (PlayerCamera == null)
            {
                Debug.LogError("Unable to locate a camera component with the MainCamera tag. ");
            }
        }

        if (PlayerController == null)
        {
            PlayerController = FindAnyObjectByType<PlayerController>();

            if (PlayerController == null)
            {
                Debug.Log("Unable to locate a PlayerController component in scene.");
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (CameraPosition == CameraPositionState.FirstPerson && FirstPersonCameraHolder != null)
        {
            // Update the pitch of the camera holder object before...
            UpdateCameraHolderPitch(FirstPersonCameraHolder, CameraPosition);
            /// ... setting the cameras position and rotation to mirror the camera holder.
            PlayerCamera.transform.SetPositionAndRotation(FirstPersonCameraHolder.position, FirstPersonCameraHolder.rotation);
        }
    }

    public void AssignTransformToFollowForCamera(Transform transformToFollow, PlayerController playerController)
    {
        PlayerController = playerController;
        FirstPersonCameraHolder = transformToFollow;
    }

    public void UpdateCameraHolderPitch(Transform cameraHolder, CameraPositionState cameraPosition)
    {
        if (cameraPosition == CameraPositionState.FirstPerson)
        {
            if (PlayerController != null && PlayerController.InputHandler != null)
            {
                CameraPitch -= PlayerController.InputHandler.RotationInput.y * Time.deltaTime * PlayerController.RotationSpeed;
                CameraPitch = Mathf.Clamp(CameraPitch, -85, 85);

                cameraHolder.transform.localRotation = Quaternion.Euler(CameraPitch, cameraHolder.transform.localRotation.y, cameraHolder.transform.localRotation.z);
            }
            else
            {
                Debug.LogError("CharacterControllerComp is null or player input handler is null");
            }
        }
    }

    /// <summary>
    /// The camera state affecting it's position and how it is rotated and controlled.
    /// </summary>
    public enum CameraPositionState
    {
        FirstPerson,
        ThirdPerson
    }
}
