using UnityEngine;

public class Values : MonoBehaviour, IDamagable
{
    [field: SerializeField]
    public StartingValueSO StartingValues
    { get; set; }

    public float Health
    { get; set; }

    public float MaxHealth 
    { get; set; }

    public void Awake()
    {
        // Init of the stats from the SO reference.
        SetValuesStats();

        // Set up the listeners for the SO to allow changes to be pushed to runtime instances that are using the SO data.
        StartingValues.UpdatedLinkedStats += SetValuesStats;
    }

    private void OnDestroy()
    {
        if (StartingValues != null)
        {
            StartingValues.UpdatedLinkedStats -= SetValuesStats;
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

    void SetValuesStats()
    {
        if (StartingValues == null)
        {
            StartingValues = ScriptableObject.CreateInstance<StartingValueSO>();
            Debug.LogWarning($"The starting values for {this.name} of type {this.GetType()} were null on awake");
        }

        MaxHealth = StartingValues.HealthMaximum;
        Health = Mathf.Clamp(StartingValues.HealthStarting, 0, StartingValues.HealthMaximum);
    }
}
