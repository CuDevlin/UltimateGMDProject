using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float baseFireRate = 0.5f;
    public float fireRateUpgradeStep = 0.05f;
    public int maxFireRateLevel = 5;

    [Header("Multishot Settings")]
    [Range(1, 5)] public int projectileCountLevel = 1;
    public float maxSpreadAngle = 45f;

    private float cooldown;
    private int fireRateLevel = 0;

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
        int count = Mathf.Clamp(projectileCountLevel, 1, 5);

        if (count == 1)
        {
            SpawnProjectile(direction);
            return;
        }

        float angleStep = count > 1 ? maxSpreadAngle / (count - 1) : 0f;
        float startAngle = -maxSpreadAngle / 2f;

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
        projectile.Launch(direction);
    }

    void Update()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }

    public void IncreaseProjectileLevel()
    {
        projectileCountLevel = Mathf.Min(projectileCountLevel + 1, 5);
        Debug.Log($"Projectile count level increased to {projectileCountLevel}");
    }

    public void IncreaseFireRateLevel()
    {
        fireRateLevel = Mathf.Min(fireRateLevel + 1, maxFireRateLevel);
        Debug.Log($"Fire rate level increased to {fireRateLevel}, new fire rate: {GetCurrentFireRate():0.00}");
    }
}
