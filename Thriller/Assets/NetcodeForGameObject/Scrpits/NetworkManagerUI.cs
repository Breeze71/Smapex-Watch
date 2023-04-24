using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBut;
    [SerializeField] private Button hostBut;
    [SerializeField] private Button clientBut;

    private void Awake() 
    {
        serverBut.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartServer();
        });

        hostBut.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartHost();
        });

        clientBut.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
