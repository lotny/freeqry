using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehavior : MonoBehaviour
{

    public int ChickensInLevel;
    public int ChickensSaved;
    private ExitBehavior exitBehavior;

    void Start()
    {
        exitBehavior = FindObjectOfType<ExitBehavior>();
        ChickensInLevel = 0;
        var chickenObjects = FindObjectsOfType<ChickenBehavior>();

        foreach(var chickenObject in chickenObjects)
        {
            if (chickenObject.isFreed == false)
            {
                ChickensInLevel += 1;
            }
        }

        ChickensSaved = 0;

      
        if (exitBehavior == null) return;
     

        if (ChickensInLevel == 0)
        {
            if (exitBehavior.IsExitOpen == false)
            {
                Debug.Log("open exit");
                exitBehavior.OpenExit();
            }
        } else {
            Debug.Log("close exit");
            exitBehavior.CloseExit();
        }
    }

    public void OnChickenSaved()
    {
        ChickensSaved += 1;
        if (ChickensSaved >= ChickensInLevel)
        {
            exitBehavior.OpenExit();
        }
    }

  


}
