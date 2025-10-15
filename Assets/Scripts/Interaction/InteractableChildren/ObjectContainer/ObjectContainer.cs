using System;
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
    public bool IsHeld
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

    public virtual void Interaction(Interactioner interactioner)
    {
        if (StoredGameObject == null)
        {
            Debug.Log("Container interacted with - it is empty");

            if (interactioner.Inventory != null)
            {
                if (interactioner.Inventory.ObjectInHand != null)
                {
                    Debug.Log("Inventory hand has an object in it");

                    if (ValidateObjectForContainer(interactioner.Inventory.ObjectInHand, out GameObject validGameObject))
                    {
                        interactioner.Inventory.TryRemoveObjectFromHand(true, false, Vector3.zero);

                        SetObjectIntoContainer(validGameObject, true, false);
                    }
                }
            }
        }
        else if (StoredGameObject != null)
        {
            Debug.Log("Container interacted with - it contains an object");

            if (interactioner.Inventory != null)
            {
                if (interactioner.Inventory.ObjectInHand == null)
                {
                    Debug.Log("Inventory hand is empty");

                    GameObject removedGameObject = RemoveObjectFromContainer(true, false);

                    interactioner.Inventory.TryAddObjectToHand(removedGameObject, out _);
                }
            }
        }
    }

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
    public void BindInput(PlayerInputHandler playerInputs)
    {
        new NotImplementedException();
    }

    public void RemoveInput(PlayerInputHandler playerInputs)
    {
        new NotImplementedException();
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
                Destroy(containedObject);
                containedObject = null;
            }
            
            if (!ShouldStartWithObjectSpawned)
            {
                Destroy(containedObject);
                containedObject = null;
            }
        }

        // If there is not assigned contained object still and yet it should have one, spawned a new one.
        if (ShouldStartWithObjectSpawned && containedObject == null)
        {
            containedObject = Instantiate(ContainedObjectPrefab, ContainerObjectPosition.position, Quaternion.identity);
        }

        // Store the singular validate object as being stored if one exisits.
        // If the object contains a gameobject with a rigidbody, then turn it kinematic to prevent the book from falling over.
        // Make sure the object itself is not interatable within the container so the interactions don't get mixed up between object being contained interaction itself and containers.
        if (containedObject != null)
        {
            SetObjectIntoContainer(containedObject, true, false);
        }
    }

    public void SetObjectIntoContainer(GameObject gameObject, bool rigidBodyKinematicStatus, bool interactiveStatus)
    {
        StoredGameObject = gameObject;

        StoredGameObject.transform.parent = ContainerObjectPosition;

        StoredGameObject.transform.SetPositionAndRotation(ContainerObjectPosition.transform.position, ContainerObjectPosition.transform.rotation);

        SetRigidBodyStatus(StoredGameObject, rigidBodyKinematicStatus);

        SetInteractableStatus(StoredGameObject, interactiveStatus);
    }

    public GameObject RemoveObjectFromContainer(bool rigidBodyKinematicStatus, bool interactiveStatus)
    {
        GameObject objectToRemove = StoredGameObject;
        StoredGameObject = null;

        objectToRemove.transform.parent = null;

        SetRigidBodyStatus(objectToRemove, rigidBodyKinematicStatus);

        SetInteractableStatus(objectToRemove, interactiveStatus);

        return objectToRemove;
    }

    public void SetRigidBodyStatus(GameObject gameObject, bool setKinematic)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = setKinematic;
        }
    }

    public void SetInteractableStatus(GameObject gameObject, bool setInteractive)
    {
        if (gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.SetInteractive(setInteractive);
        }
    }
}
