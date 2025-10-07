using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field: SerializeField]
    public GameObject ObjectInHand
    {  get; private set; }

    [field: SerializeField]
    public Transform HandLocation
    { get; private set; }

    public void Awake()
    {
        if (HandLocation == null)
        {
            if (Interactioner.ReturnTaggedTransform("HandTransform", transform, out Transform taggedTransform))
            {
                HandLocation = taggedTransform;
            }
            else
            {
                Debug.LogError("The passed transform parent does not contain a child with the passed tag. No HandLocation transform aqquired. Defaulting to this transform.");
                HandLocation = transform;
            }
        }
    }

    public void TryAddObjectToHand(GameObject objectToAdd)
    {
        if (ObjectInHand != null)
        {
            Debug.LogWarning("Hand already contained object.");
            return;
        }

        AddObjectToHand(objectToAdd);
    }

    public void AddObjectToHand(GameObject objectToAdd)
    {
        // Add the object to the handlocation in physical space and into it's heirarchy.
        objectToAdd.transform.parent = HandLocation;
        objectToAdd.transform.SetPositionAndRotation(HandLocation.transform.position, HandLocation.transform.rotation);

        // Once the object is in hand, disable any physics for the Rigidbody if present.
        if (objectToAdd.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }

        // Once the object is in hand, disable any interaction oppertunity for the IInteractable if present.
        if (objectToAdd.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.SetInteractive(false);
        }

        // Add the object to the inventory.
        ObjectInHand = objectToAdd;
    }

    public void TryRemoveObjectFromHand(bool enablePhysicsForRigidBody, bool enableInteractionForInteractable, Vector3 positionToPlace)
    {
        if (ObjectInHand == null)
        {
            Debug.LogWarning("Hand does not contain a object.");
            return;
        }

        RemoveObjectToHand(enablePhysicsForRigidBody, enableInteractionForInteractable, positionToPlace);
    }

    public void RemoveObjectToHand(bool enablePhysicsForRigidBody, bool enableInteractionForInteractable, Vector3 positionToPlace)
    {
        // Remove the object from the handlocation and its heirarchy.
        ObjectInHand.transform.parent = null;
        ObjectInHand.transform.SetPositionAndRotation(positionToPlace, Quaternion.identity);

        // Once the object is out of hand, choose to re-enable any physics for the Rigidbody if present.
        if (ObjectInHand.TryGetComponent<Rigidbody>(out Rigidbody rigidbody) && enablePhysicsForRigidBody)
        {
            rigidbody.isKinematic = false;
        }

        // Once the object is out of hand, choose to re-enable interaction oppertunity for the IInteractable if present.
        if (ObjectInHand.TryGetComponent<IInteractable>(out IInteractable interactable) && enableInteractionForInteractable)
        {
            interactable.SetInteractive(true);
        }

        // Remove the object from the inventory.
        ObjectInHand = null;
    }


}
