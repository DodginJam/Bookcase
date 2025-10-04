using UnityEngine;
using static IInteractable;

[DefaultExecutionOrder(-1)]
public class Book : MonoBehaviour, IInteractable
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

    void Awake()
    {
        IsInterationAllowed = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
