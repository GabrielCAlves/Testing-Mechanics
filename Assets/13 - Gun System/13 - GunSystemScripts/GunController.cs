using UnityEngine;

public class GunController : MonoBehaviour
{
    public GunData currentGun;
    public Transform firePoint;        // Onde a bala sai (ponta do cano)
    public Transform cameraTransform;  // A câmera do jogador
    public Vector3 targetPoint;
    public float bulletSpeed = 5f;
    public bool independentCameraFromCharacter = false;

    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading;
    private SimpleFSM simpleFSM;

    //void Start() => currentAmmo = currentGun.maxAmmo;
    void Start()
    {
        currentAmmo = currentGun.maxAmmo;
        simpleFSM = GetComponentInParent<SimpleFSM>();
    }

    void Update()
    {
        if (isReloading) return;

        // Lógica de tiro (automático ou semi)
        if (currentGun.isAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1"))
        {
            if (currentAmmo > 0 && Time.time >= nextFireTime)
            {
                if (simpleFSM != null)
                {
                    simpleFSM.SetSingleShot();
                }
                Fire();
            }
            else if (currentAmmo <= 0)
            {
                if (simpleFSM != null)
                {
                    simpleFSM.SetReload();
                }
                StartReload();
            }
                
        }

        if (Input.GetKeyDown(KeyCode.R)) StartReload();

        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 dir = GetDirectionWithSpread();
                Debug.DrawRay(firePoint.position, dir * 20f, Color.yellow, 0.1f);
            }
        }
    }

    // --- FIRE CORRIGIDO PARA SHOTGUN ---
    void Fire()
    {
        nextFireTime = Time.time + currentGun.fireRate;
        currentAmmo--;

        // 1. Executa o Tiro (Hitscan ou Projétil)
        if (currentGun.bulletPrefab != null)
        {
            if (currentGun.gunName == "Shotgun") // Ou use uma bool isShotgun
            {
                for (int i = 0; i < 8; i++)
                {
                    // CADA pellet tem seu próprio spread!
                    Vector3 pelletDirection = GetDirectionWithSpread();
                    FireProjectile(pelletDirection);
                }
            }
            else
            {
                Vector3 aimDirection = GetDirectionWithSpread();
                FireProjectile(aimDirection);
            }
        }
        else
        {
            Vector3 aimDirection = GetDirectionWithSpread();
            FireHitscan(aimDirection);
        }

        // 2. Efeitos (Muzzle Flash, Recuo)
        if (currentGun.muzzleFlashPrefab != null)
            Instantiate(currentGun.muzzleFlashPrefab, firePoint.position, firePoint.rotation);

        ApplyRecoil();
    }

    // --- CÁLCULO DA MIRA COM SPREAD (VERSÃO CORRIGIDA FINAL) ---
    Vector3 GetDirectionWithSpread()
    {
        // 1. Usa ScreenPointToRay para pegar o centro exato da tela
        //    (mais preciso que ViewportPointToRay para jogos)

        //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //targetPoint = ray.GetPoint(currentGun.range);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        RaycastHit hit;
        Vector3 targetPoint;

        // 2. Verifica se o raio da câmera colidiu com algo
        if (Physics.Raycast(ray, out hit, currentGun.range))
        {
            targetPoint = hit.point; // Acertou um objeto (inimigo, parede, etc)
        }
        else
        {
            // Não acertou nada, usa um ponto no infinito na direção do crosshair
            targetPoint = ray.GetPoint(currentGun.range);
        }

        // 3. Calcula a direção do CANO da arma até o ponto alvo
        Vector3 baseDirection = (targetPoint - firePoint.position).normalized;

        if(independentCameraFromCharacter) 
            gameObject.transform.LookAt(new Vector3(0, targetPoint.y, 0));

        // 4. Se spread for 0, retorna a direção exata (sem nenhum offset)
        if (currentGun.spread <= 0)
        {
            return baseDirection;
        }

        // 5. Aplica o spread (se houver)
        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 cameraUp = Camera.main.transform.up;

        float spreadX = Random.Range(-currentGun.spread, currentGun.spread);
        float spreadY = Random.Range(-currentGun.spread, currentGun.spread);

        Vector3 spreadOffset = (cameraRight * spreadX) + (cameraUp * spreadY);
        Vector3 finalDirection = (baseDirection + spreadOffset).normalized;

        return finalDirection;
    }

    // --- HITSCAN (Bala instantânea) ---
    void FireHitscan(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, direction, out hit, currentGun.range))
        {
            // Aplica dano ao alvo (exemplo simples)
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(currentGun.damage);

            // Opcional: Linha de traço visual (VFX)
            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.1f);
        }
    }

    // --- PROJÉTIL (Bala física) ---
    void FireProjectile(Vector3 direction)
    {
        GameObject bullet = Instantiate(currentGun.bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed; // Velocidade do projétil
        // Passa o dano e dono para o projétil (via script na bala)
    }

    void ApplyRecoil()
    {
        // Move a câmera para trás e para cima (simulando recuo)
        cameraTransform.Rotate(-currentGun.recoilForce, 0, 0);
        // Ou use um Vector3.Lerp para animar suavemente
    }

    void StartReload()
    {
        if (currentAmmo == currentGun.maxAmmo || isReloading) return;
        isReloading = true;
        Invoke(nameof(FinishReload), currentGun.reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = currentGun.maxAmmo;
        isReloading = false;
        if (simpleFSM != null)
        {
            simpleFSM.SetIdle();
        }
    }
}