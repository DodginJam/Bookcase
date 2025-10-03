using System.Collections.Generic;
using UnityEngine;

public class Interactioner : MonoBehaviour
{
    [field: SerializeField]
    public Transform InteractionTransform
    { get; private set; }

    private void Awake()
    {
        if (InteractionTransform == null)
        {
            if (ReturnTaggedTransform("CameraHolder", transform, out Transform taggedTransform))
            {
                InteractionTransform = taggedTransform;
            }
            else
            {
                Debug.LogError("The passed transform parent does not contain a child with the passed tag. No interaction transform aqquired. Defaulting to this transform.");
                InteractionTransform = transform;
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

    public void Interact()
    {
        if (Physics.Raycast(InteractionTransform.position, InteractionTransform.forward, out RaycastHit hit))
        {
            // First check if their is a rigidbody on the hit, and if there is use that as the point to locate an interactable interface.
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.TryInteraction(transform, hit);
                }

                Debug.Log("Rigidbody found with collider hit.");
            }
            // Else just look directly at the collider to check for whether the object contains an interatable interface.
            else
            {
                if (hit.collider.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.TryInteraction(transform, hit);
                }

                Debug.Log("Just collider hit - no rigidbody");
            }
        }
    }

    bool ReturnTaggedTransform(string tag, Transform parentTransform, out Transform taggedTransform)
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
}
