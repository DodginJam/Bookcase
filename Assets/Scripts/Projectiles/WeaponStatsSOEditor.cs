using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponStatsSO))]
public class WeaponStatsSOEditor : Editor
{
    public WeaponStatsSO WeaponSO
    {  get; set; }

    public SerializedProperty FireModeStateProp
    { get; set; }

    private SerializedProperty ChargeTimeProp
    { get; set; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Push Changes At Runtime"))
        {
            WeaponSO.UpdateLinkedWeaponValues();
        }

        if ((FireMode)FireModeStateProp.enumValueIndex == FireMode.Charge)
        {
            EditorGUILayout.LabelField("Charge Fire Mode Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(ChargeTimeProp);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        WeaponSO = (WeaponStatsSO)target;

        FireModeStateProp = serializedObject.FindProperty("<FireModeState>k__BackingField");

        ChargeTimeProp = serializedObject.FindProperty("<ChargeTime>k__BackingField");

    }
}
