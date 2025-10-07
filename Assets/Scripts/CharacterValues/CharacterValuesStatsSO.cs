using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterValuesStatsSO", menuName = "Create New CharacterValuesStatsSO")]
public class CharacterValuesStatsSO : ScriptableObject
{
    [field: SerializeField]
    public CharacterValuesSO MaximumStats
    { get; set; }

    [field: SerializeField]
    public CharacterValuesSO StartingStats
    { get; set; }
}
