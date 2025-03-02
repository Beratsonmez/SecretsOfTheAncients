using UnityEngine;
using Mirror;

public interface IInteractable
{
    void Interact(NetworkIdentity player); //Etkile�ime ge�en oyuncu.
    string GetInteractionPrompt(); // Log i�erisinde g�sterilecek mesaj
    bool CanInteract(NetworkIdentity player); //Etkile�ime ge�ilebilir mi ?
    void Interact();

}
