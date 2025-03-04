using UnityEngine;
using Mirror;
public class WeightBall : NetworkBehaviour
{
    [SyncVar]
    public float weight = 1.0f;

    void Start()
    {
        if (!isServer) return;
    }

}
