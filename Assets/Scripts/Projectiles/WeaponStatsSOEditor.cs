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
    private SerializedProperty BurstNumberOfShotsProp
    { get; set; }

    private SerializedProperty BurstShotFireRateProp
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
        else if ((FireMode)FireModeStateProp.enumValueIndex == FireMode.Burst)
        {
            EditorGUILayout.LabelField("Burst Fire Mode Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(BurstNumberOfShotsProp);

            EditorGUILayout.PropertyField(BurstShotFireRateProp);
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        WeaponSO = (WeaponStatsSO)target;

        FireModeStateProp = serializedObject.FindProperty("<FireModeState>k__BackingField");

        ChargeTimeProp = serializedObject.FindProperty("<ChargeTime>k__BackingField");

        BurstNumberOfShotsProp = serializedObject.FindProperty("<BurstNumberOfShots>k__BackingField");

        BurstShotFireRateProp = serializedObject.FindProperty("<BurstShotFireRate>k__BackingField");
    }
}
