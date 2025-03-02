using UnityEngine;
using Mirror;

public interface IInteractable
{
    void Interact(NetworkIdentity player); //Etkileþime geçen oyuncu.
    string GetInteractionPrompt(); // Log içerisinde gösterilecek mesaj
    bool CanInteract(NetworkIdentity player); //Etkileþime geçilebilir mi ?
    void Interact();

}
