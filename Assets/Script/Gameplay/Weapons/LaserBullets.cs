using UnityEngine;

public class LaserBullets : MonoBehaviour
{
    private Vector2 moveDirection;
    private float bulletSpeed;
    private float bulletDamage;

    public void Initialize(Vector2 shootDirection, float speed, float damage)
    {
        this.moveDirection = shootDirection.normalized;
        this.bulletSpeed = speed;
        this.bulletDamage = damage;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * bulletSpeed * Time.deltaTime);

        if (transform.position.x > 55) // batas luar layar kanan
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah objek yang ditabrak adalah obstacle, hancurkan peluru tanpa damage.
        if (other.CompareTag("obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        // REFAKTOR: Cek apakah objek yang ditabrak memiliki komponen shield (apapun jenisnya)
        // dengan mencari antarmuka IShield.
        IShield shield = other.GetComponent<IShield>();
        if (shield != null)
        {
            // Shield akan menyerap damage.
            int remainingDamage = shield.AbsorbDamage((int)this.bulletDamage);

            // Jika ada sisa damage setelah shield menyerap, teruskan ke badan musuh.
            if (remainingDamage > 0)
            {
                // Cari komponen IDamageable di parent (badan musuh).
                IDamageable underlyingEnemy = other.GetComponentInParent<IDamageable>();
                if (underlyingEnemy != null)
                {
                    underlyingEnemy.TakeDamage(remainingDamage);
                }
            }

            Destroy(gameObject);
            return;
        }

        // Cek apakah objek yang ditabrak bisa menerima damage (implementasi IDamageable).
        // Ini akan bekerja untuk SEMUA musuh, termasuk Boss1, Boss2, dan Boss3.
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage((int)this.bulletDamage);
            Destroy(gameObject);
            return;
        }
    }

}
