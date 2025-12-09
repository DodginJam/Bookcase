using UnityEngine;

public class SceneCamera : MonoBehaviour
{
    [field: SerializeField]
    public CameraController AssignedCameraController
    {  get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
    }
}
