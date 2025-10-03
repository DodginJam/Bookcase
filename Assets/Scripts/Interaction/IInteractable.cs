using UnityEngine;

public interface IInteractable
{
    public float InteractionDistance
    {  get; set; }

    public bool IsInterationAllowed
    { get; set; }

    public InteractionCentrePoint InteractionCentre
    { get; set; }

    public void TryInteraction(Transform interactionerTransform, RaycastHit hitInformation)
    {
        if (IsInterationAllowed == false)
        {
            return;
        }

        if (Vector3.Distance(interactionerTransform.position, GetInteractionCentrePoint(hitInformation)) > InteractionDistance)
        {
            return;
        }

        Interact();
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

    public abstract void Interact();

    public enum InteractionCentrePoint
    {
        TransformCentre,
        RayHitPoint
    }
}
