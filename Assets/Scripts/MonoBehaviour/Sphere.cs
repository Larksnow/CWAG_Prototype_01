using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sphere : MonoBehaviour, ISelectable
{
    public QuickOutline.Outline outline;
    void Start()
    {
        outline.enabled = false;
    }
    public void BeingSelect()
    {
        outline.enabled = true;
    }
}
