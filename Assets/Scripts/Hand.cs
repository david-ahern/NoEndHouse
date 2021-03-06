﻿using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour 
{
    private Item EquippedItem;

    public RigidbodyConstraints EquippedConstraints;
    private RigidbodyConstraints UnequippedContraints;

    public bool IsEquipped
    {
        get { return (EquippedItem == null ? false : true); }
    }

    public void Interact()
    {
        RaycastHit Hit;

        if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Globals.Player.Reach))
        {
            if (!IsEquipped && Hit.collider.tag == "Item")
                EquipItem(Hit.collider.gameObject);
            else if (IsEquipped && Hit.collider.tag == "ItemHolder")
                Hit.collider.gameObject.GetComponent<ItemHolder>().PlaceItem(GiveItem());
            else if (Hit.collider.tag == "Window")
                Hit.collider.gameObject.GetComponent<Window>().ToggleOpen();
            else if (IsEquipped)
                DropItem();
        }
        else if (IsEquipped)
            DropItem();
    }

    public void EquipItem(GameObject item)
    {
        EquippedItem = item.GetComponent<Item>();

        if (EquippedItem.InHolder)
            EquippedItem.transform.parent.gameObject.GetComponent<ItemHolder>().RemoveItem();

        EquippedItem.InHolder = false;
        EquippedItem.gameObject.transform.parent = gameObject.transform;

        EquippedItem.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        EquippedItem.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        EquippedItem.gameObject.collider.enabled = false;
        EquippedItem.gameObject.rigidbody.useGravity = false;
        EquippedItem.gameObject.rigidbody.isKinematic = true;

        SoundController.PlayClip(EquippedItem.Clip);
    }

    public void DropItem()
    {
        EquippedItem.gameObject.transform.parent = Globals.Player.CurrentArea.transform;

        EquippedItem.gameObject.collider.enabled = true;
        EquippedItem.gameObject.rigidbody.useGravity = true;
        EquippedItem.gameObject.rigidbody.isKinematic = false;
        
        EquippedItem = null;
    }

    public Item GiveItem()
    {
        Item temp = EquippedItem;
        EquippedItem = null;
        return temp;
    }
}
