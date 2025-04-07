using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    public float fireRate = 0.5f;

    private float cooldown;

    public void TryFire(Vector2 direction)
    {
        if (cooldown <= 0f)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position + (Vector3)(direction.normalized * 0.5f), Quaternion.identity);
            Projectile projectile = proj.GetComponent<Projectile>();
            projectile.Launch(direction.normalized);

            cooldown = fireRate;
        }
    }

    void Update()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }
}