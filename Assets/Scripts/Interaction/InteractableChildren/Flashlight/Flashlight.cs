using UnityEngine;
using static IInteractable;

public class Flashlight : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public float InteractionDistance
    { get; set; }

    [field: SerializeField]
    public bool IsInterationAllowed
    { get; set; }

    [field: SerializeField]
    public InteractionCentrePoint InteractionCentre
    { get; set; }

    [field: SerializeField]
    public Light LightEmitter
    { get; set; }
    public bool IsHeld
    { get; set; }

    void Awake()
    {
        IsInterationAllowed = true;


        if (LightEmitter == null)
        {
            LightEmitter = GetComponentInChildren<Light>();

            if (LightEmitter == null)
            {
                Debug.LogWarning("The flashlight does not have a light component within it's children.");
            }
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

    public void ToggleLight()
    {
        LightEmitter.enabled = !LightEmitter.enabled;
    }

    public void BindInput(PlayerInputHandler playerInputHandler)
    {
        playerInputHandler.ToggleFlashlightPress += ToggleLight;
    }
    public void RemoveInput(PlayerInputHandler playerInputHandler)
    {
        playerInputHandler.ToggleFlashlightPress += ToggleLight;
    }

    public void Interaction(Interactioner interactioner)
    {
        if (interactioner.Inventory != null)
        {
            interactioner.Inventory.TryAddObjectToHand(this.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (IsInterationAllowed)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}
