using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button _startHost;
    [SerializeField] private Button _startClient;
    [SerializeField] private Button _shutDown;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _cameraOffline;

    private void Start()
    {
        _startHost.onClick.AddListener(StartHost);
        _startClient.onClick.AddListener(StartClient);
        _shutDown.onClick.AddListener(Shutdown);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientLostConnection;
        }

        UpdateStatusText();
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientLostConnection;
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdateStatusText();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdateStatusText();
    }

    private void OnClientLostConnection(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.LogWarning("Disconnected from server!");

            _statusText.text = "Status: Disconnected from Server";
            _cameraOffline.SetActive(true);
            UpdateButtonsInteractable();
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        UpdateStatusText();
    }

    public void StartClient()
    {
        var unityTransport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (unityTransport != null)
        {
            unityTransport.ConnectionData.Address = "127.0.0.1";
        }

        NetworkManager.Singleton.StartClient();
        UpdateStatusText();
    }

    public void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
        StartCoroutine(DelayedUpdate());
    }

    private IEnumerator DelayedUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            _statusText.text = "Status: Offline";
            _cameraOffline.SetActive(true);
            UpdateButtonsInteractable();
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            _statusText.text = "Status: Host";
            _cameraOffline.SetActive(false);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                _statusText.text = "Status: Client (Connected)";
                _cameraOffline.SetActive(false);
            }
            else
            {
                _statusText.text = "Status: Client (Connecting...)";
                _cameraOffline.SetActive(true);
            }
        }
        else
        {
            _statusText.text = "Status: Unknown";
            _cameraOffline.SetActive(true);
        }

        UpdateButtonsInteractable();
    }

    private void UpdateButtonsInteractable()
    {
        if (NetworkManager.Singleton == null)
        {
            _startHost.interactable = true;
            _startClient.interactable = true;
            _shutDown.interactable = false;
            return;
        }

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            _startHost.interactable = false;
            _startClient.interactable = false;
            _shutDown.interactable = true;
        }
        else
        {
            _startHost.interactable = true;
            _startClient.interactable = true;
            _shutDown.interactable = false;
        }
    }
}