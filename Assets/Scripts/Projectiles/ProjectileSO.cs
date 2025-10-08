using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileSO", menuName = "Create New ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    [field: SerializeField, Min(0)]
    public float Damage
    { get; private set; } = 20.0f;

    [field: SerializeField, Min(0.1f)]
    public float Force
    { get; private set; } = 20.0f;

    [field: SerializeField, Min(0.1f)]
    public float Mass
    { get; private set; } = 20.0f;

    [field: SerializeField, Min(0.01f)]
    public float ActiveLifeTime
    { get; private set; } = 0.1f;

    public event Action UpdateLinkedProjectiles;

    public void UpdateLinkedProjectileValues()
    {
        UpdateLinkedProjectiles?.Invoke();
    }
}