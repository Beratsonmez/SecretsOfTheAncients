using Mirror;
using UnityEngine;

public class InteractableObject : NetworkBehaviour, IInteractable
{
    [SyncVar] // De�i�ikliklerin istemcilere g�nderilmesini sa�lar
    private bool isInteracted = false;

    public bool CanInteract(NetworkIdentity player)
    {
        throw new System.NotImplementedException();
    }

    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Interact()
    {
        if (isServer) // Sadece sunucu de�i�tirebilir
        {
            isInteracted = !isInteracted;
            RpcHandleInteraction();
        }
    }

    public void Interact(NetworkIdentity player)
    {
        throw new System.NotImplementedException();
    }

    [ClientRpc]
    private void RpcHandleInteraction()
    {
        Debug.Log($"{gameObject.name} ile etkile�ime girildi!");
    }
}
