using UnityEngine;
using Mirror;

public interface IGrabbable
{
    void Grab(Transform attachPoint);
    void Drop(Vector3 dropPosition, Vector3 dropForce);
}
