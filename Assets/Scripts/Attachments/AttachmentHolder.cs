using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using UnityEngine;

public class AttachmentHolder : MonoBehaviour
{
    [field: SerializeField]
    public List<AttachmentData<Flashlight>> FlashlightList
    { get; private set; } = new List<AttachmentData<Flashlight>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveBindingsFromAttachments(PlayerInputHandler playerInputHandler)
    {
        foreach (AttachmentData<Flashlight> lightSlot in FlashlightList)
        {
            if (lightSlot.AttachmentGameObject != null)
            {
                lightSlot.AttachmentGameObject.RemoveInput(playerInputHandler);
            }
        }
    }

    public void AddBindingsFromAttachments(PlayerInputHandler playerInputHandler)
    {
        foreach (AttachmentData<Flashlight> lightSlot in FlashlightList)
        {
            if (lightSlot.AttachmentGameObject != null)
            {
                lightSlot.AttachmentGameObject.BindInput(playerInputHandler);
            }
        }
    }

    public bool CheckForEmptySlot<T>(List<AttachmentData<T>> attachmentList, out AttachmentData<T> attachmentData)
    {
        attachmentData = null;

        if (attachmentList != null && attachmentList.Count > 0)
        {
            for (int i = 0; i < attachmentList.Count; i++)
            {
                if (attachmentList[i].TransformParent == null)
                {
                    Debug.LogError("The transform for the attachment has been left null.");
                    return false;
                }

                if (attachmentList[i].AttachmentGameObject == null)
                {
                    Debug.Log("There is space for the attachment found.");
                    attachmentData = attachmentList[i];
                    return true;
                }
            }
        }

        return false;
    }

    public void AddAttachmentToSlot<T>(AttachmentData<T> attachmentDataToAddToo, T attachmentData)
    {
        attachmentDataToAddToo.AttachmentGameObject = attachmentData;
    }

    public void RemoveAttachmentFromSlot<T>(List<AttachmentData<T>> listOfAttachments, T objectToRemove)
    {
        List<AttachmentData<T>> query = listOfAttachments.Where(element => EqualityComparer<T>.Default.Equals(element.AttachmentGameObject, objectToRemove)).Select(element => element).ToList();

        foreach (AttachmentData<T> data in query)
        {
            data.AttachmentGameObject = default(T);
        }
    }

    public void SetAttachmentToParent(Transform parentTransform, GameObject attachmentGameObject)
    {
        attachmentGameObject.transform.parent = parentTransform;
        attachmentGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}

[Serializable]
public class AttachmentData<T> 
{
    [field: SerializeField]
    public Transform TransformParent
    { get; set; }

    [field: SerializeField]
    public T AttachmentGameObject
    { get; set; }
}
