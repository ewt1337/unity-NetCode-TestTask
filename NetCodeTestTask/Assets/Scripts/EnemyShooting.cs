using Unity.Netcode;
using UnityEngine;

public class EnemyShooting : NetworkBehaviour
{
    [SerializeField] private NetworkObject _bulletPrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _shootInterval = 2f;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _shootRange = 15f;

    private float _shootTimer;
    private Transform _target;

    private void Update()
    {
        if (!IsHost) return;

        _shootTimer += Time.deltaTime;
        if (_shootTimer >= _shootInterval)
        {
            FindTarget();
            if (_target != null)
            {
                Shoot();
            }
            _shootTimer = 0f;
        }
    }

    private void FindTarget()
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var clientObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if (clientObject == null) continue;

            float distance = Vector3.Distance(transform.position, clientObject.transform.position);
            if (distance < closestDistance && distance <= _shootRange)
            {
                closestDistance = distance;
                closestPlayer = clientObject.gameObject;
            }
        }

        _target = closestPlayer != null ? closestPlayer.transform : null;
    }

    private void Shoot()
    {
        if (_bulletPrefab == null || _shootPoint == null || _target == null) return;

        NetworkObject bulletInstance = Instantiate(_bulletPrefab, _shootPoint.position, Quaternion.identity);
        bulletInstance.Spawn();

        Vector3 direction = (_target.position - _shootPoint.position).normalized;

        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, _bulletSpeed);
        }
    }
}
