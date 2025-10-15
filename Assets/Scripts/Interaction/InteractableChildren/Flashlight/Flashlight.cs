using UnityEngine;
using static IInteractable;

public class Flashlight : MonoBehaviour, IInteractable, IAttachment
{
    [field: SerializeField]
    public float InteractionDistance
    { get; set; }

    [field: SerializeField]
    public bool IsInterationAllowed
    { get; set; }

    [field: SerializeField]
    public InteractionCentrePoint InteractionCentre
    { get; set; }

    [field: SerializeField]
    public Light LightEmitter
    { get; set; }

    public bool IsHeld
    { get; set; }

    public AttachmentHolder AttachmentHolderLinkedToo
    { get; set; }

    public PlayerInputHandler PlayerInputHandlerLinkedToo
    { get; set; }

    void Awake()
    {
        IsInterationAllowed = true;


        if (LightEmitter == null)
        {
            LightEmitter = GetComponentInChildren<Light>();

            if (LightEmitter == null)
            {
                Debug.LogWarning("The flashlight does not have a light component within it's children.");
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

    public void ToggleLight()
    {
        LightEmitter.enabled = !LightEmitter.enabled;
    }

    public void BindInput(PlayerInputHandler playerInputHandler)
    {
        playerInputHandler.ToggleFlashlightTap += ToggleLight;
        playerInputHandler.DropFlashLightHold += DropFlashLightAsAttachment;
    }

    public void RemoveInput(PlayerInputHandler playerInputHandler)
    {
        playerInputHandler.ToggleFlashlightTap -= ToggleLight;
        playerInputHandler.DropFlashLightHold -= DropFlashLightAsAttachment;
    }

    void DropFlashLightAsAttachment()
    {
        if (AttachmentHolderLinkedToo == null || PlayerInputHandlerLinkedToo == null)
        {
            return;
        }

        this.gameObject.transform.parent = null;

        if (this.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
        }

        if (this.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.SetInteractive(true);

            interactable.RemoveInput(PlayerInputHandlerLinkedToo);
            interactable.IsHeld = false;
        }

        AttachmentHolderLinkedToo.RemoveAttachmentFromSlot<Flashlight>(AttachmentHolderLinkedToo.FlashlightList, this);

        PlayerInputHandlerLinkedToo = null;
        AttachmentHolderLinkedToo = null;
    }

    public void Interaction(Interactioner interactioner)
    {
        if (interactioner.Inventory != null)
        {
            GameObject objectInHand;

            // If an object can not be added to the hand, check the gameobject already in the hand.
            if (!interactioner.Inventory.TryAddObjectToHand(this.gameObject, out objectInHand))
            {
                // Check if the gameObject is an attachment holder which can be added to.
                if (objectInHand.TryGetComponent<AttachmentHolder>(out AttachmentHolder attachmentHolder))
                {
                    // Is there space for a attachment of the type flashlight?
                    if (attachmentHolder.CheckForEmptySlot<Flashlight>(attachmentHolder.FlashlightList, out AttachmentData<Flashlight> attachmentData))
                    {
                        AttachmentHolderLinkedToo = attachmentHolder;

                        // With space being found, add the flashlight data to the slot and set it's position.
                        attachmentHolder.AddAttachmentToSlot<Flashlight>(attachmentData, this);
                        attachmentHolder.SetAttachmentToParent(attachmentData.TransformParent, this.gameObject);

                        // Once the object is attached, disable any physics for the Rigidbody if present.
                        if (this.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                        {
                            rigidbody.isKinematic = true;
                        }

                        // Once the object is attached, disable any interaction oppertunity for the IInteractable if present.
                        if (this.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                        {
                            interactable.SetInteractive(false);

                            if (interactioner.TryGetComponent<PlayerInputHandler>(out PlayerInputHandler playerInput))
                            {
                                PlayerInputHandlerLinkedToo = playerInput;

                                interactable.BindInput(playerInput);
                                interactable.IsHeld = true;
                            }
                        }
                    }
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
