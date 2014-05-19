using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class ItemHolder : MonoBehaviour 
{
    public Item PlacedItem;

    public bool HasItem
    {
        get { return (PlacedItem == null ? false : true); }
    }

    public void PlaceItem(Item item)
    {
        Debug.Log("Placing Item");
        PlacedItem = item;
        PlacedItem.gameObject.transform.parent = gameObject.transform;
        PlacedItem.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        PlacedItem.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        PlacedItem.InHolder = true;
    }
}
