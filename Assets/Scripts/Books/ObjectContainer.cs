using UnityEngine;
using static IInteractable;

public abstract class ObjectContainer<T> : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public float InteractionDistance
    { get; set; }

    public bool IsInterationAllowed
    { get; set; }

    [field: SerializeField]
    public InteractionCentrePoint InteractionCentre
    { get; set; }

    [field: SerializeField]
    public GameObject ObjectContained
    {  get; private set; }

    [field: SerializeField]
    public Transform ContainerObjectPosition
    { get; private set; }

    public abstract void Interact();

    public bool ValidateObjectForContainer(GameObject objectToCheck)
    {
        if (objectToCheck.TryGetComponent<T>(out _))
        {
            Debug.Log("Valid object to be contained.");
            return true;
        }
        else
        {
            Debug.Log("Not a valid object to be contained.");
            return false;
        }
    }

    void Awake()
    {
        IsInterationAllowed = true;

        if (ContainerObjectPosition == null)
        {
            Debug.LogWarning("No transform for object container location provided, transform created from this.transform.");

            GameObject newGameObject = new GameObject($"{this.gameObject.name} contained object transform");
            ContainerObjectPosition = newGameObject.transform;
            ContainerObjectPosition.transform.position = this.transform.position;
            ContainerObjectPosition.parent = this.transform;
        }

        if (ObjectContained == null)
        {
            /*
            if (transform.GetComponentInChildren<T>(out T objecttoContain))
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
            */
        }

        // If the object contains a gameobject with a rigidbody, then turn it kinematic to prevent the book from falling over.
        if (ObjectContained != null)
        {
            if (ObjectContained.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
