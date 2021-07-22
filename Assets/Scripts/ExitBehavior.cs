using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehavior : MonoBehaviour
{
    public bool IsExitOpen;
    private SpriteRenderer ExitTooltipSR;

    void Start()
    {
        ExitTooltipSR = transform.Find("ExitTooltip").gameObject.GetComponent<SpriteRenderer>();
       
       
    }

    public void CloseExit()
    {
        if (ExitTooltipSR == null)
        {
            ExitTooltipSR = transform.Find("ExitTooltip").gameObject.GetComponent<SpriteRenderer>();
        }
        ExitTooltipSR.color = new Color(0, 0, 0, 0);
        IsExitOpen = false;
    }

    public void OpenExit()
    {
        if (ExitTooltipSR == null)
        {
            ExitTooltipSR = transform.Find("ExitTooltip").gameObject.GetComponent<SpriteRenderer>();
        }
        ExitTooltipSR.color = new Color(255, 255, 255, 1);
        IsExitOpen = true;
    }

    
}
