using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _lifetime = 5f;
    
    private Vector3 _direction;
    private float _speed;
    private float _timer;

    public void Initialize(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
    }

    private void Update()
    {
        if (!IsOwner) return;

        transform.position += _direction * _speed * Time.deltaTime;
        _timer += Time.deltaTime;

        if (_timer >= _lifetime)
        {
            DespawnBullet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }

        DespawnBullet();
    }

    private void DespawnBullet()
    {
        if (IsSpawned)
        {
            Destroy(gameObject);
        }
    }
}