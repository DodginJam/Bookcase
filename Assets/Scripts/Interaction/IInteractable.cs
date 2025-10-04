using UnityEngine;

public interface IInteractable
{
    public float InteractionDistance
    {  get; set; }

    public bool IsInterationAllowed
    { get; set; }

    public InteractionCentrePoint InteractionCentre
    { get; set; }

    public void TryInteraction(RaycastHit hitInformation, Interactioner interactioner)
    {
        if (IsInterationAllowed == false)
        {
            return;
        }

        if (Vector3.Distance(interactioner.transform.position, GetInteractionCentrePoint(hitInformation)) > InteractionDistance)
        {
            return;
        }

        Interaction(interactioner);
    }

    public Vector3 GetInteractionCentrePoint(RaycastHit hitInformation)
    {
        Vector3 interactionCentre = Vector3.zero;

        if (InteractionCentre == InteractionCentrePoint.TransformCentre)
        {
            interactionCentre = hitInformation.transform.position;
        }
        else if (InteractionCentre == InteractionCentrePoint.RayHitPoint)
        {
            interactionCentre = hitInformation.point;
        }

        return interactionCentre;
    }

    public void SetInteractive(bool isInteractionAllowed)
    {
        IsInterationAllowed = isInteractionAllowed;
    }

    public abstract void Interaction(Interactioner interactioner);

    public enum InteractionCentrePoint
    {
        TransformCentre,
        RayHitPoint
    }
}
