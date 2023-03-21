using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    shotgun,
    Other
}
public enum BurstType
{
    singleFire,
    burstFire,
    autoFire
}
[CreateAssetMenu(menuName ="weaponObjects", order = 1)]
public class Wep_obj : ScriptableObject
{
    public string weaponName;
    [Header("bullet sttings")]
    public int bulletDamage;
    public float bulletRange;
    public LayerMask bulletMask;
    [Header("Firerate")]
    public float fireRate;
    [Tooltip("only works when burstFire active")] public float burstFirerate;
    [Header("FireRate type options")]
    [Tooltip("only applies if burstFire is active")] public int burstAmount;
    public float burstCooldown;
    public BurstType burstType;
    public WeaponType weaponType;
}
