using UnityEngine;
using static IInteractable;

public class Book : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public float InteractionDistance
    { get; set; }

    public bool IsInterationAllowed
    { get; set; }

    [field: SerializeField]
    public InteractionCentrePoint InteractionCentre
    { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsInterationAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("Book interacted with.");
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
