using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]

    
    public class ShowItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Awake(){
            slotUI = GetComponent<SlotUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        { 
            if (slotUI.itemDetails != null)
            {
                inventoryUI.ItemTooltip.gameObject.SetActive(true);
                inventoryUI.ItemTooltip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);
                inventoryUI.ItemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.ItemTooltip.transform.position = transform.position+Vector3.up*120;
            }
            else{
                inventoryUI.ItemTooltip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.ItemTooltip.gameObject.SetActive(false);
        }



    }
}
