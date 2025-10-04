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
    public GameObject ContainedObjectPrefab
    {  get; private set; }

    [field: SerializeField]
    public Transform ContainerObjectPosition
    { get; private set; }

    [field: SerializeField]
    public bool ShouldStartWithObjectSpawned
    { get; set; }

    public GameObject StoredGameObject
    { get; private set; }

    public abstract void Interact();

    public bool ValidateObjectForContainer(GameObject objectToCheck, out GameObject validatedGameObject)
    {
        bool validated = false;

        if (objectToCheck.TryGetComponent<T>(out _))
        {
            Debug.Log("Valid object to be contained.");
            validated = true;
        }
        else
        {
            Debug.Log("Not a valid object to be contained.");
            validated = false;
        }

        validatedGameObject = validated ? objectToCheck : null;
        return validated;
    }

    void Awake()
    {
        IsInterationAllowed = true;

        // Ensure that there is a transform container for the position of the contained game object to be placed under / at.
        if (ContainerObjectPosition == null)
        {
            Debug.LogWarning("No transform for object container location provided, transform created from this.transform.");

            GameObject newGameObject = new GameObject($"{this.gameObject.name} contained object transform");
            ContainerObjectPosition = newGameObject.transform;
            ContainerObjectPosition.transform.position = this.transform.position;
            ContainerObjectPosition.parent = this.transform;
        }

        InitialiseAndValidateContainedObjectOnAwake();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void InitialiseAndValidateContainedObjectOnAwake()
    {
        // Validate the circumstances for the contained object to always appear as a single instantiated gameobject starting at the transform point if it is set to spawn or if an valid object has already been manually placed within.
        GameObject containedObject = null;

        // If there is more then one object in the container, remove them.
        if (ContainerObjectPosition.childCount > 1)
        {
            Debug.LogWarning("More then one object has already been placed within the object container.");

            GameObject[] childObjects = ContainerObjectPosition.GetComponentsInChildren<GameObject>();

            foreach (GameObject objectChild in childObjects)
            {
                Destroy(objectChild);
            }
        }

        // If an object already is within the container transform heirarachy, validate it should belong there and assign it as the contained object if valid.
        if (ContainerObjectPosition.childCount == 1)
        {
            // Grab the object that is already contained within the container.
            containedObject = ContainerObjectPosition.GetChild(0).gameObject;

            if (!containedObject.TryGetComponent<T>(out _))
            {
                Debug.LogError("Object in container is not containing the correct script type to be stored here.");
                Destroy(containedObject);
                containedObject = null;
            }
            else
            {
                containedObject.transform.SetPositionAndRotation(ContainerObjectPosition.transform.position, ContainerObjectPosition.transform.rotation);
            }
        }

        // If there is not assigned contained object still and yet it should have one, spawned a new one.
        if (ShouldStartWithObjectSpawned && containedObject == null)
        {
            containedObject = Instantiate(ContainedObjectPrefab, ContainerObjectPosition.position, Quaternion.identity, ContainerObjectPosition);
        }

        // Store the singular validate object as being stored if one exisits.
        // If the object contains a gameobject with a rigidbody, then turn it kinematic to prevent the book from falling over.
        // Make sure the object itself is not interatable within the container so the interactions don't get mixed up between object being contained interaction itself and containers.
        if (containedObject != null)
        {
            StoredGameObject = containedObject;

            if (StoredGameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
            }

            if (StoredGameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.IsInterationAllowed = false;
            }
        }
    }
}
