using UnityEngine;

[CreateAssetMenu(fileName = "New SemiAutoSO", menuName = "Create New SemiAutoSO")]
public class SemiAutoSO : WeaponTypeSO
{
    public override void OnFirePressed(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireProjectile();
        }
    }
}
