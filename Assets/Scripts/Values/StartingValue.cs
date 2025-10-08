using UnityEngine;

[CreateAssetMenu(fileName = "new Starting ValueSO", menuName = "Starting ValueSO")]
public class StartingValue : ScriptableObject
{
    [field: SerializeField, Min(0)]
    public float Health
    { get; set; } = 100;
}
