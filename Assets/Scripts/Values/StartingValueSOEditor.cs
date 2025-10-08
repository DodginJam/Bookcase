using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StartingValueSO))]
public class StartingValueSOEditor : Editor
{
    public StartingValueSO StartingValueSO
    { get; private set; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Push Changes At Runtime"))
        {
            StartingValueSO.PushUpdateLinkedStats();
        }
    }

    private void OnEnable()
    {
        StartingValueSO = (StartingValueSO)target;
    }
}
