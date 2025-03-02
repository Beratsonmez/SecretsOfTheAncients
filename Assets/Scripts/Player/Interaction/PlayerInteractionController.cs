using UnityEngine;
using Mirror;

public class PlayerInteractionController : NetworkBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform grabPoint; // Oyuncunun eli

    private Camera playerCamera;
    private GameObject heldObject = null;

    public override void OnStartAuthority()
    {
        AssignCamera();
    }

    private void Start()
    {

        if (interactableLayer == 0)
        {
            interactableLayer = LayerMask.GetMask("Placement", "Interactable", "Grabbable");
        }

    }

    private void Update()
    {
        if (!isOwned || playerCamera == null) return; // E�er bu bizim karakterimiz de�ilse, i�lem yapma

        if (Input.GetKeyDown(KeyCode.E))
        {
            
            if (heldObject == null)
            {
                TryInteract();
            }
            else
            {
                DropItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && heldObject != null)
        {
            TryPlaceItem();
        }
    }

    private void AssignCamera()
    {
        playerCamera = GetComponentInChildren<Camera>(); // Oyuncu i�indeki kameray� bul

        if (playerCamera == null)
        {
            Debug.LogError("PlayerInteractionController: FPS Kameras� bulunamad�! L�tfen Player prefab'�nda bir kamera oldu�undan emin olun.");
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            // E�er PlacementPoint'e bak�yorsak ve i�inde obje varsa, al
            if (hit.collider.TryGetComponent(out PlacementPoint placementPoint))
            {
                GameObject retrievedObject = placementPoint.RemoveObject();
                if (retrievedObject != null)
                {
                    CmdGrabObject(retrievedObject);
                    return;
                }
            }

            // E�er ta��nabilir bir obje ise, al
            if (hit.collider.TryGetComponent(out IGrabbable grabbable))
            {
                CmdGrabObject(hit.collider.gameObject);
                return;
            }

            // E�er ba�ka bir etkile�im varsa (buton, kap� vs.)
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void TryPlaceItem()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            
            Debug.Log($"Raycast temas etti: {hit.collider.gameObject.name}");

            if (hit.collider.TryGetComponent(out PlacementPoint placementPoint))
            {
                Debug.Log("PlacementPoint bulundu!");
                if (placementPoint.CanPlace())
                {
                    Debug.Log($"Yerle�tirilecek nesne: {heldObject.name}");
                    CmdPlaceObject(heldObject, placementPoint.gameObject);
                }
                else
                {
                    Debug.Log("PlacementPoint dolu!");
                }
            }
            else
            {
                Debug.Log("PlacementPoint bulunamad�!");
            }
        }
        else
        {
            Debug.Log("Raycast hi�bir �eye �arpmad�!");
        }
    }



    [ClientRpc] // T�m istemcilerde etkile�imi �al��t�r
    private void RpcInteract(GameObject interactableObject)
    {
        IInteractable interactable = interactableObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private void DropItem()
    {
        if (heldObject != null)
        {
            Vector3 dropPosition = grabPoint.position + playerCamera.transform.forward * 1.5f;
            Vector3 dropForce = playerCamera.transform.forward * 2f;
            CmdDropObject(heldObject, dropPosition, dropForce);
        }
    }

    [Command]
    private void CmdPlaceObject(GameObject obj, GameObject placementPointObj)
    {
        Debug.Log($"CmdPlaceObject �a�r�ld�: {obj.name} -> {placementPointObj.name}");

        if (obj.TryGetComponent(out GrabbableObject grabbable) &&
            placementPointObj.TryGetComponent(out PlacementPoint placementPoint))
        {
            placementPoint.PlaceObject(obj);
            heldObject = null;
            Debug.Log("Nesne sunucuda yerle�tirildi!");
        }
        else
        {
            Debug.Log("Yerle�tirme ba�ar�s�z: Objeler bulunamad�!");
        }
    }


    [Command] // Sunucuya etkile�imi bildir
    private void CmdInteract(GameObject interactableObject)
    {
        if (interactableObject != null)
        {
            RpcInteract(interactableObject);
        }
    }

    [Command]
    private void CmdGrabObject(GameObject obj)
    {
        if (obj.TryGetComponent(out GrabbableObject grabbable))
        {
            grabbable.Grab(grabPoint);
            heldObject = obj;
        }
    }

    [Command]
    private void CmdDropObject(GameObject obj, Vector3 dropPosition, Vector3 dropForce)
    {
        if (obj.TryGetComponent(out GrabbableObject grabbable))
        {
            grabbable.Drop(dropPosition, dropForce);
            heldObject = null;
        }
    }
}

