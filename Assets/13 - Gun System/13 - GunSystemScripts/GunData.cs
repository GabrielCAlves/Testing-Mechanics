using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Scriptable Objects/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;
    public GameObject bulletPrefab; // Deixe vazio para Hitscan
    public float fireRate = 0.15f;   // Segundos entre tiros
    public int damage = 10;
    public float range = 100f;
    public float spread = 0.5f;      // Ângulo de dispersão (0 é sniper, 5 é shotgun)
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public bool isAutomatic = false;

    // Efeitos (Muzzle Flash, Som, Recuo)
    public GameObject muzzleFlashPrefab;
    public float recoilForce = 2f;
}