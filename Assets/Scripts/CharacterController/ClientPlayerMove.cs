using UnityEngine;
using Unity.Netcode;

public class ClientPlayerMove : NetworkBehaviour
{
    [field: SerializeField]
    public PlayerInputHandler InputHandler
    {  get; private set; }

    [field: SerializeField]
    public PlayerController PlayerController
    { get; private set; }

    void Awake()
    {
        InputHandler.enabled = false;
        PlayerController.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            InputHandler.enabled = true;
            PlayerController.enabled = true;
        }
    }
}
