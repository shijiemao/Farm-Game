using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public List<NPCPosition> npcPositionList;
    public SceneRouteDataList_SO sceneRouteDate;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                Debug.Log(key);

                if (sceneRouteDict.ContainsKey(key))
                    continue;
                else
                    sceneRouteDict.Add(key, route);
                
            }

        }
        
    }

    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
