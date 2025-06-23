using UnityEngine;
using Unity.Netcode;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private NetworkObject _targetNetworkObject;
    [SerializeField] private Vector3 _offset = new Vector3(0, 5, -7);
    [SerializeField] private float _smoothSpeed = 0.125f;

    private void Start()
    {
        if (_targetNetworkObject == null || !_targetNetworkObject.IsLocalPlayer)
        {
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_targetNetworkObject == null || !_targetNetworkObject.IsLocalPlayer) return;

        Vector3 desiredPosition = _targetNetworkObject.transform.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(_targetNetworkObject.transform);
    }
}