using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactioner : MonoBehaviour
{
    public PlayerInputHandler InputHandler
    { get; private set; }

    [field: SerializeField]
    public Transform InteractionTransform
    { get; private set; }

    public Inventory Inventory
    { get; private set; }

    [field: SerializeField, Min(1f)]
    public float DropObjectDistance
    { get; private set; } = 3f;

    [field: SerializeField]
    public bool DisplayDebugGizmos
    { get; private set; }

    private void Awake()
    {
        if (InteractionTransform == null)
        {
            if (ReturnTaggedTransform("InteractionFace", transform, out Transform taggedTransform))
            {
                InteractionTransform = taggedTransform;
            }
            else
            {
                Debug.LogError("The passed transform parent does not contain a child with the passed tag. No interaction transform aqquired. Defaulting to this transform.");
                InteractionTransform = transform;
            }
        }

        if (Inventory == null)
        {
            if (TryGetComponent<Inventory>(out Inventory inventory))
            {
                Inventory = inventory;
            }
            else
            {
                Inventory = this.AddComponent<Inventory>();
            }
        }

        if (InputHandler == null)
        {
            if (TryGetComponent<PlayerInputHandler>(out PlayerInputHandler inputHandler))
            {
                InputHandler = inputHandler;
            }
            else
            {
                Debug.LogError("Unable to locate the input handler on this component.");
            }
        }
    }

    private void OnEnable()
    {
        AssignEventListeners();
    }

    private void OnDisable()
    {
        DeassignEventListeners();
    }

    public void AssignEventListeners()
    {
        if (InputHandler != null)
        {
            InputHandler.InteractionTap += InteractRayCast;
            InputHandler.InteractionHold += DropObject;
        }
    }

    public void DeassignEventListeners()
    {
        if (InputHandler != null)
        {
            InputHandler.InteractionTap -= InteractRayCast;
            InputHandler.InteractionHold -= DropObject;
        }
    }

    public void InteractRayCast()
    {
        if (Physics.Raycast(ReturnRay(), out RaycastHit hit))
        {
            // First check if there is a rigidbody on the hit, and if there is use that as the transform to locate an interactable interface.
            // Else just look directly at the collider to check for whether the transform contains an interatable interface.
            Transform hitTransformToPass = (hit.rigidbody != null) ? hit.rigidbody.transform : hit.collider.transform;

            if (hitTransformToPass.TryGetComponent(out IInteractable interactable))
            {
                interactable.TryInteraction(hit, this);
            }
        }
    }

    public Ray ReturnRay()
    {
        Ray ray = new Ray(InteractionTransform.position, InteractionTransform.forward);

        Debug.DrawRay(InteractionTransform.position, InteractionTransform.forward, Color.white, 0.5f);

        return ray;
    }

    public void DropObject()
    {
        if (Inventory != null)
        {
            if (Inventory.ObjectInHand != null)
            {
                if (Physics.Raycast(ReturnRay(), out RaycastHit hit, DropObjectDistance))
                {                                                               // Adds a little bit of room to prevent ground clipping.
                    Inventory.TryRemoveObjectFromHand(true, true, hit.point + (Vector3.up * 0.25f));
                }
                else
                {
                    Inventory.TryRemoveObjectFromHand(true, true, Inventory.ObjectInHand.transform.position);
                }
            }
        }
        else
        {
            Debug.LogError("The inventory reference has not been assigned.");
        }
    }

    public static bool ReturnTaggedTransform(string tag, Transform parentTransform, out Transform taggedTransform)
    {
        Transform child = null;
        bool childFound = false;

        // Error checks for the passed string tag.
        if (string.IsNullOrWhiteSpace(tag) || tag == string.Empty)
        {
            Debug.LogError("A null, white space or empty string are not valid tags.");
            taggedTransform = child;
            return childFound;
        }

        // Null check for the transform parent.
        if (parentTransform == null)
        {
            Debug.LogError("The transform parent passed is null.");
            taggedTransform = child;
            return childFound;
        }

        // Loop through all the child objects and check if any have the tag.
        Transform[] childTransformArray = parentTransform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < childTransformArray.Length; i++)
        {
            if (childTransformArray[i].CompareTag(tag))
            {
                child = childTransformArray[i];
                childFound = true;
                break;
            }
        }

        // Set the out Transform and return the found status.
        taggedTransform = child;
        return childFound;
    }

    private void OnDrawGizmos()
    {
        if (DisplayDebugGizmos != true)
        {
            return;
        }

        if (InteractionTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(InteractionTransform.position, DropObjectDistance);
        }
    }
}
