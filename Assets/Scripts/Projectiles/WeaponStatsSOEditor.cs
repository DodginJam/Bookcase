using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponStatsSO))]
public class WeaponStatsSOEditor : Editor
{
    public WeaponStatsSO WeaponSO
    {  get; set; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Push Changes At Runtime"))
        {
            WeaponSO.UpdateLinkedWeaponValues();
        }
    }

    private void OnEnable()
    {
        WeaponSO = (WeaponStatsSO)target;
    }
}
