using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("ItemData")]
        public ItemDataList_SO itemDataList_SO;

        [Header("BagData")]
        public InventoryBag_SO playerBag;

        private void OnEnable(){
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
        }

        private void OnDisable() {
            EventHandler.DropItemEvent -=OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
        }


        ///when game start update the record bag data

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);

        }

        private void OnHarvestAtPlayerPosition(int ID)
        {
            var index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// return ItemDetail by itemID
        /// </summary>
        /// <param name = "ID">Item ID</param>
        /// <returns></returns>

        public ItemDetails GetItemDetails(int ID) {
            return itemDataList_SO.itemDetailsList.Find(i=>i.itemID == ID);
        }

        /// <summary>
        /// return ItemDetail by itemID
        /// </summary>
        /// <param name = "ID">Item ID</param>
        /// <returns></returns>

        public void AddItem(Item item, bool toDestroy)
        {
            //whether bag already has the item
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            if (toDestroy)
            {
                Destroy(item.gameObject);
            }

            //update UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// check whether bag has room
        /// </summary>
        /// <param name = "ID">Item ID</param>
        /// <returns></returns>

 
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0){
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// find the id of bag item
        /// </summary>
        /// <param name = "ID">Item ID</param>
        /// <returns></returns>
        
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i <playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// find the id of bag item
        /// </summary>
        /// <param name = "ID">Item ID</param>
        /// <param name = "ID">index</param>
        /// <param name = "amount">amount</param>
        /// <returns></returns>
        

        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 &&CheckBagCapacity())
            //if not in bag, place the item in the first blank room
            {
                var item = new InventoryItem{itemID = ID, itemAmount = amount};
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0){
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            // if  already in bag, add the amount to the current.
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem{itemID = ID, itemAmount = currentAmount};
                playerBag.itemList[index] = item;
            }
        }

        //swap item in a same bag
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();

            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }


        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

    }
}    
