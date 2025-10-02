using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputHandler InputHandler
    {  get; private set; }

    public CharacterController CharacterControllerComp
    {  get; private set; }

    [field: SerializeField]
    public float MovementSpeed
    { get; private set; } = 1.0f;

    [field: SerializeField]
    public float RotationSpeed
    { get; private set; } = 1.0f;

    private void Awake()
    {
        InitialiseVariables();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    public void InitialiseVariables()
    {
        if (TryGetComponent<CharacterController>(out CharacterController characterController))
        {
            CharacterControllerComp = characterController;
        }
        else
        {
            Debug.LogError("Unable to locate a character controller component on this transform.");
        }

        if (TryGetComponent<PlayerInputHandler>(out PlayerInputHandler inputHandler))
        {
            InputHandler = inputHandler;
        }
        else
        {
            Debug.LogError("Unable to locate a player input handler component on this transform.");
        }
    }

    public void HandleMovement()
    {
        if (CharacterControllerComp != null & InputHandler != null)
        {
            Vector3 gloablMovement = new Vector3(InputHandler.MovementInput.x, 0, InputHandler.MovementInput.y);

            CharacterControllerComp.Move(transform.TransformDirection(gloablMovement) * Time.deltaTime * MovementSpeed);
        }
        else
        {
            Debug.LogError("CharacterControllerComp is null or player input handler is null");
        }
    }

    public void HandleRotation()
    {
        if (CharacterControllerComp != null & InputHandler != null)
        {
            Vector3 globalRotation = new Vector3(0, InputHandler.RotationInput.x, 0);

            transform.Rotate(globalRotation * Time.deltaTime * RotationSpeed);
        }
        else
        {
            Debug.LogError("CharacterControllerComp is null or player input handler is null");
        }
    }
}
