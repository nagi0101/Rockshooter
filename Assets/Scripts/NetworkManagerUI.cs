using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button ClientButton;


    // Start is called before the first frame update
    void Start()
    {
        HostButton.onClick.AddListener(() =>
        {
            NetworkManager.StartHost();
        });
        ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.StartServer();
        });
        ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.StartClient();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
