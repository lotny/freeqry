using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderBehavior : MonoBehaviour
{
    private GameObject tooltip;

    private void Start()
    {
        tooltip = gameObject.transform.GetChild(0).gameObject;
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void ShowTooltip()
    {
        tooltip.SetActive(true);

    }

    public void FlipToolTipUp()
    {
        tooltip.GetComponent<SpriteRenderer>().flipY = true;
    }

    public void FlipToolTipDown()
    {
        tooltip.GetComponent<SpriteRenderer>().flipY = false;
    }



}
