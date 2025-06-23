using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    private Transform _target;

    private float _targetSearchInterval = 1f;
    private float _targetSearchTimer;

    private float _stopDistance = 3f;
    private float _rotationSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            _agent.enabled = false;
            enabled = false;
        }
    }

    private void Update()
    {
        if (!IsHost) return;

        _targetSearchTimer += Time.deltaTime;
        if (_targetSearchTimer >= _targetSearchInterval)
        {
            FindClosestPlayer();
            _targetSearchTimer = 0f;
        }

        if (_target != null)
        {
            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance > _stopDistance)
            {
                _agent.SetDestination(_target.position);
            }
            else
            {
                _agent.ResetPath();
            }

            RotateTowardsTarget();
        }
        else
        {
            _agent.ResetPath();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        direction.y = 0;

        if (direction.magnitude == 0) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    private void FindClosestPlayer()
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var clientObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if (clientObject == null) continue;

            float distance = Vector3.Distance(transform.position, clientObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = clientObject.gameObject;
            }
        }

        if (closestPlayer != null)
        {
            _target = closestPlayer.transform;
        }
        else
        {
            _target = null;
            _agent.ResetPath();
        }
    }
}