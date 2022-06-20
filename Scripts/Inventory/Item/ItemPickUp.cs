using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MFarm.Inventory

{
    public class ItemPickUp : MonoBehaviour

    {
        // Start is called before the first frame update
        private void OnTriggerEnter2D(Collider2D other) {
            Item item = other.GetComponent<Item>();

            if (item != null){
                if (item.itemDetails.canPickedup)
                {
                    //Add item to bag
                    InventoryManager.Instance.AddItem(item, true);
                }
            }
        }
    }
}

