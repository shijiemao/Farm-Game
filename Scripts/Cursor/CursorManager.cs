using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.Map;
using MFarm.CropPlant;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;
    private ItemDetails currentItem;
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }
    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }    
        else
        {
            SetCursorImage(normal);
        }    
    }

    private void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();

    }
    #region set cursor
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1,1,1,1);
    }
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1,1,1,1);
    }
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1,0,0,0.4f);
    }
    #endregion


    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        
        if(!isSelected)
        {
            currentItem = null;
            currentSprite = normal;
            cursorEnable = false;
        }
        else
        {
            currentItem = itemDetails;
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.BreakTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.CollectTool => tool,
                ItemType.ReapTool => tool,
                _ => normal
            };
            cursorEnable = true;
        } 
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        // check whether in use radius

        if(Mathf.Abs(mouseGridPos.x - playerGridPos.x)>currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y)>currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if(currentTile !=null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            switch(currentItem.itemType)
            {
                case ItemType.Commodity:
                    if(currentTile.canDropItem  && currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if(currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if(currentTile.daySinceDig > -1 && currentTile.daySinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Seed:
                    if (currentTile.daySinceDig > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInValid();
                    break;  
                case ItemType.CollectTool:
                    if(currentCrop!= null)
                    {
                        if(currentCrop.CheckToolAvailable(currentItem.itemID))
                            if(currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else
                        SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }

        // Debug.Log("WorldPos:"+ mouseWorldPos + "  GridPos" + mouseGridPos);
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
