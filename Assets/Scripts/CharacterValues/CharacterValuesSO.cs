using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterValuesSO", menuName = "Create New CharacterValuesSO")]
public class CharacterValuesSO : ScriptableObject
{
    [field: SerializeField, Min(0)]
    public float Health
    { get; set; }
}
