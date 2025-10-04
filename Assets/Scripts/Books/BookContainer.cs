using System;
using UnityEngine;

public class BookContainer : ObjectContainer<Book>
{
    public override void Interact()
    {
        if (StoredGameObject == null)
        {
            Debug.Log("Book Container interacted with - it is empty");
        }
        else
        {
            Debug.Log("Book Container interacted with - it is full");
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
