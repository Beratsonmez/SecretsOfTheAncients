using UnityEngine;

public class PlacementPoint : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    private GameObject placedObject;

    public bool CanPlace()
    {
        Debug.Log($"CanPlace() çaðrýldý, IsOccupied: {IsOccupied}");
        return !IsOccupied;
    }

    public void PlaceObject(GameObject obj)
    {
        if (CanPlace())
        {
            Debug.Log($"Nesne yerleþtirildi: {obj.name} -> {gameObject.name}");

            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.SetParent(transform);
            IsOccupied = true;
            placedObject = obj;
        }
        else
        {
            Debug.Log("Yerleþtirme baþarýsýz: Nokta dolu!");
        }
    }

    public GameObject RemoveObject()
    {
        if (placedObject != null)
        {
            GameObject obj = placedObject;
            obj.transform.SetParent(null);
            placedObject = null;
            IsOccupied = false;
            return obj;
        }
        return null;
    }
}
