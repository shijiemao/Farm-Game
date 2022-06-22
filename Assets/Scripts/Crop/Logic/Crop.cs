using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    public bool CanHarvest =>tileDetails.growthDays>=cropDetails.TotalGrowthDays;
    private int harvestActionCount;
    private Animator anim;
    private Transform PlayerTransform=>FindObjectOfType<Player>().transform;

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        //check whether animation
        anim = GetComponentInChildren<Animator>();

        //click count
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount ++;
            //Effect and Voice
            if(anim!=null && cropDetails.hasAnimation)
            {
                if(PlayerTransform.position.x < transform.position.x)
                {
                    anim.SetTrigger("RotateRight");
                }
                else
                {
                    anim.SetTrigger("RotateLeft");
                }
                
            }
            if (cropDetails.hasParticalEffect) 
                EventHandler.CallParticaleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);

        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)//create commodity
            {
                SpawnHarvestItems();
            }
            else if(cropDetails.hasAnimation)
            {
                if(PlayerTransform.position.x < transform.position.x)
                {
                    anim.SetTrigger("FallingRight");
                }
                else{
                    anim.SetTrigger("FallingLeft");
                }
                StartCoroutine(HarvestAfterAnimation());

            }
        }
    }
    private IEnumerator HarvestAfterAnimation()
    {
        while(!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }
        SpawnHarvestItems();
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }

    }

    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daysSinceLastHarvest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();
    }

    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;
            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }

            for (int j = 0; j < amountToProduce; j ++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }

                else //create item in world
                {
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1: -1;
                    var spawnPos = new Vector3(transform.position.x +Random.Range(dirX,cropDetails.spawnRadius.x*dirX),
                    transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i],spawnPos);
                }
            }
        }
        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;

            //whether regrow
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTime)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                EventHandler.CallRefreshCurrentMap();

            }
            else
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
            }
            Destroy(gameObject);
        }
    }
}