using UnityEngine;

public class Values : MonoBehaviour, IDamagable
{
    [field: SerializeField]
    public StartingValue StartingValues
    { get; set; }

    [field: SerializeField, Min(0)]
    public float Health
    { get; set; }

    [field: SerializeField, Min(0)]
    public float MaxHealth 
    { get; set; }

    public void Awake()
    {
        if (StartingValues == null)
        {
            StartingValues = ScriptableObject.CreateInstance<StartingValue>();
            Debug.LogWarning($"The starting values for {this.name} of type {this.GetType()} were null on awake");
        }

        MaxHealth = StartingValues.Health;
        Health = StartingValues.Health;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
