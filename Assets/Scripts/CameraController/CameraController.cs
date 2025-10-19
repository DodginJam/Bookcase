using UnityEngine;

[DefaultExecutionOrder(50)]
public class CameraController : MonoBehaviour
{
    public Camera AttachedCamera
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
    public PlayerController PlayerControllerOwner
    { get; private set; }

    private void Awake()
    {
        if (TryGetComponent<Camera>(out Camera camera))
        {
            AttachedCamera = camera;
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
            AttachedCamera.transform.SetPositionAndRotation(FirstPersonCameraHolder.position, FirstPersonCameraHolder.rotation);
        }
    }

    public void AssignTransformToFollowForCamera(Transform transformToFollow, PlayerController playerController)
    {
        PlayerControllerOwner = playerController;
        FirstPersonCameraHolder = transformToFollow;
    }

    public void UpdateCameraHolderPitch(Transform cameraHolder, CameraPositionState cameraPosition)
    {
        if (cameraPosition == CameraPositionState.FirstPerson)
        {
            if (PlayerControllerOwner != null && PlayerControllerOwner.InputHandler != null)
            {
                CameraPitch -= PlayerControllerOwner.InputHandler.RotationInput.y * Time.deltaTime * PlayerControllerOwner.RotationSpeed;
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
