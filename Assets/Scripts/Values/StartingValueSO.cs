using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new Starting ValueSO", menuName = "Starting ValueSO")]
public class StartingValueSO : ScriptableObject
{
    [field: SerializeField, Min(0)]
    public float HealthStarting
    { get; set; } = 100;

    [field: SerializeField, Min(0)]
    public float HealthMaximum
    { get; set; } = 200;

    public event Action UpdatedLinkedStats;

    public void PushUpdateLinkedStats()
    {
        UpdatedLinkedStats?.Invoke();
    }
}
