using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private NetworkVariable<int> _health = new NetworkVariable<int>(100);

    public int Health => _health.Value;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _health.Value = 100;
        }
    }
    
    private void OnEnable()
    {
        _health.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"Health changed: {oldHealth} -> {newHealth}");
    }

    public void TakeDamage(int damage)
    {
        if (!IsOwner) return;

        _health.Value = Mathf.Max(_health.Value - damage, 0);
        
        if (_health.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        NetworkManager.Singleton.Shutdown();
    }
}