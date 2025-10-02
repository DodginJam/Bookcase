using UnityEngine;

[DefaultExecutionOrder(-2)]
public class InputManager : MonoBehaviour
{
    public InputSystem_Actions InputActions
    {  get; private set; }

    private void Awake()
    {
        if (InputActions == null)
        {
            InputActions = new InputSystem_Actions();
            InputActions.Enable();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
