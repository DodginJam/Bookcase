using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectileSO))]
public class ProjectileSOEditor : Editor
{
    public ProjectileSO Projectile
    {  get; set; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Push Changes To All Projectiles"))
        {
            Projectile.UpdateLinkedProjectileValues();
        }
    }

    private void OnEnable()
    {
        Projectile = (ProjectileSO)target;
    }
}
