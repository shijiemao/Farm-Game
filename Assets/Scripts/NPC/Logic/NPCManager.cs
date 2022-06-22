using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<NPCPosition> npcPositionList;
    public SceneRouteDataList_SO sceneRouteDate;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    private void Awake()
    {

    }

    private void InitSceneRouteDict()
    {
        
    }
}
