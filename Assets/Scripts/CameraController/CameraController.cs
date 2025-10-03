using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera PlayerCamera
    {  get; private set; }

    [field: SerializeField]
    public Transform FirstPersonCameraHolder
    { get; private set; }

    public float CameraPitch
    { get; private set; }

    public CameraPositionState CameraPosition
    { get; private set; }

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
                Debug.LogError("Unable to locate a PlayerController component in scene.");
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
        if (CameraPosition == CameraPositionState.FirstPerson)
        {
            PlayerCamera.transform.SetPositionAndRotation(FirstPersonCameraHolder.position, FirstPersonCameraHolder.rotation);
            UpdateCameraRotation(FirstPersonCameraHolder, CameraPosition);
        }
    }

    public enum CameraPositionState
    {
        FirstPerson,
        ThirdPerson
    }

    public void UpdateCameraRotation(Transform cameraHolder, CameraPositionState cameraPosition)
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
}
