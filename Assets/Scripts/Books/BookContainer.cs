using System;
using UnityEngine;

public class BookContainer : ObjectContainer<Book>
{
    public override void Interaction(Interactioner interactioner)
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

                    interactioner.Inventory.TryAddObjectToHand(removedGameObject);
                }
            }
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
