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
        if (!isOwned || playerCamera == null) return; // Eðer bu bizim karakterimiz deðilse, iþlem yapma

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
        playerCamera = GetComponentInChildren<Camera>(); // Oyuncu içindeki kamerayý bul

        if (playerCamera == null)
        {
            Debug.LogError("PlayerInteractionController: FPS Kamerasý bulunamadý! Lütfen Player prefab'ýnda bir kamera olduðundan emin olun.");
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            // Eðer PlacementPoint'e bakýyorsak ve içinde obje varsa, al
            if (hit.collider.TryGetComponent(out PlacementPoint placementPoint))
            {
                GameObject retrievedObject = placementPoint.RemoveObject();
                if (retrievedObject != null)
                {
                    CmdGrabObject(retrievedObject);
                    return;
                }
            }

            // Eðer taþýnabilir bir obje ise, al
            if (hit.collider.TryGetComponent(out IGrabbable grabbable))
            {
                CmdGrabObject(hit.collider.gameObject);
                return;
            }

            // Eðer baþka bir etkileþim varsa (buton, kapý vs.)
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
                    Debug.Log($"Yerleþtirilecek nesne: {heldObject.name}");
                    CmdPlaceObject(heldObject, placementPoint.gameObject);
                }
                else
                {
                    Debug.Log("PlacementPoint dolu!");
                }
            }
            else
            {
                Debug.Log("PlacementPoint bulunamadý!");
            }
        }
        else
        {
            Debug.Log("Raycast hiçbir þeye çarpmadý!");
        }
    }



    [ClientRpc] // Tüm istemcilerde etkileþimi çalýþtýr
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
        Debug.Log($"CmdPlaceObject çaðrýldý: {obj.name} -> {placementPointObj.name}");

        if (obj.TryGetComponent(out GrabbableObject grabbable) &&
            placementPointObj.TryGetComponent(out PlacementPoint placementPoint))
        {
            placementPoint.PlaceObject(obj);
            heldObject = null;
            Debug.Log("Nesne sunucuda yerleþtirildi!");
        }
        else
        {
            Debug.Log("Yerleþtirme baþarýsýz: Objeler bulunamadý!");
        }
    }


    [Command] // Sunucuya etkileþimi bildir
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

