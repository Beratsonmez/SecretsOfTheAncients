using UnityEngine;
using Mirror;

public interface IInteractable
{
    void Interact(NetworkIdentity player); //Etkileşime geçen oyuncu.
    string GetInteractionPrompt(); // Log içerisinde gösterilecek mesaj
    bool CanInteract(NetworkIdentity player); //Etkileşime geçilebilir mi ?
    void Interact();

}
