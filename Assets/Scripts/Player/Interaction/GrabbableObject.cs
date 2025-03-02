using UnityEngine;
using Mirror;

public class GrabbableObject : NetworkBehaviour, IGrabbable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform attachPoint)
    {
        if (isServer)
        {
            rb.isKinematic = true;
            transform.position = attachPoint.position;
            transform.rotation = attachPoint.rotation;
            transform.SetParent(attachPoint);
        }
    }

    public void Drop(Vector3 dropPosition, Vector3 dropForce)
    {
        if (isServer)
        {
            transform.SetParent(null);
            rb.isKinematic = false;
            transform.position = dropPosition;
            rb.AddForce(dropForce, ForceMode.Impulse);
        }
    }


}
