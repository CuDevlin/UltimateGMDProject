using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float baseFireRate = 0.6f;
    public float fireRateUpgradeStep = 0.05f;
    public int maxFireRateLevel = 10;

    [Header("Multishot Settings")]
    [Range(1, 10)] public int projectileCountLevel = 1;
    public int maxProjectileCountLevel = 10;
    public float maxSpreadAngle = 90f;

    private float cooldown;
    private int fireRateLevel = 0;

    private void Update()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }

    public void TryFire(Vector2 direction)
    {
        if (cooldown <= 0f)
        {
            FireProjectiles(direction.normalized);
            cooldown = GetCurrentFireRate();
        }
    }

    private float GetCurrentFireRate()
    {
        return Mathf.Max(0.1f, baseFireRate - fireRateLevel * fireRateUpgradeStep);
    }

    private void FireProjectiles(Vector2 direction)
    {
        int count = Mathf.Clamp(projectileCountLevel, 1, maxProjectileCountLevel);

        if (count == 1)
        {
            SpawnProjectile(direction);
            return;
        }

        float spread = Mathf.Min(maxSpreadAngle, projectileCountLevel * 10f);
        float angleStep = (count > 1) ? spread / (count - 1) : 0f;
        float startAngle = -spread / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * direction;
            SpawnProjectile(rotatedDirection);
        }
    }

    private void SpawnProjectile(Vector2 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position + (Vector3)(direction * 0.5f), Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Launch(direction);
        }
    }

    public void IncreaseProjectileLevel()
    {
        projectileCountLevel = Mathf.Min(projectileCountLevel + 1, maxProjectileCountLevel);
        Debug.Log($"[PROJECTILE] Projectile count level increased to {projectileCountLevel}");
    }

    public void IncreaseFireRateLevel()
    {
        fireRateLevel = Mathf.Min(fireRateLevel + 1, maxFireRateLevel);
        Debug.Log($"[FIRE RATE] Fire rate level increased to {fireRateLevel}, new rate: {GetCurrentFireRate():0.00}s");
    }
}
