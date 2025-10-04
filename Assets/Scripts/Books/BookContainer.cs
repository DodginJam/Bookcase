using System;
using UnityEngine;

public class BookContainer : ObjectContainer<Book>
{
    public override void Interaction(Interactioner interactioner)
    {
        if (StoredGameObject == null)
        {
            Debug.Log("Book Container interacted with - it is empty");

            if (interactioner.Inventory != null)
            {
                Debug.Log("Inventory hand has an object in it");

                if (ValidateObjectForContainer(interactioner.Inventory.ObjectInHand, out GameObject validGameObject))
                {
                    SetObjectIntoContainer(validGameObject, true, false);
                }
            }
        }
        else
        {

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
