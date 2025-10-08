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
}