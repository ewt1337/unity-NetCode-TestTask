using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject _enemyPrefab;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private BoxCollider _spawnAreaCollider;

    private float _timer;
    private bool _isSpawning;

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            _isSpawning = true;
            _timer = 0f;
        }
    }

    public override void OnNetworkDespawn()
    {
        _isSpawning = false;
    }

    private void Update()
    {
        if (!IsHost || !_isSpawning) return;

        _timer += Time.deltaTime;
        if (_timer >= _spawnInterval)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (!IsHost) return;

        if (_enemyPrefab == null) return;

        Vector3 randomPosition = GetRandomPointInCollider(_spawnAreaCollider);
        NetworkObject enemyInstance = Instantiate(_enemyPrefab, randomPosition, Quaternion.identity);
        enemyInstance.Spawn();
    }

    private Vector3 GetRandomPointInCollider(BoxCollider collider)
    {
        Vector3 center = collider.bounds.center;
        Vector3 size = collider.bounds.size;

        float randomX = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float randomY = center.y;
        float randomZ = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(randomX, randomY, randomZ);
    }
}