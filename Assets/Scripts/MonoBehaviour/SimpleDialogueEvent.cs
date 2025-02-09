using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleCustomer : MonoBehaviour
{
    public static SimpleCustomer main;
    public bool isactive = false;
    public GameObject dialogueBox;
    private void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isactive)
        {
            isactive = true;
            dialogueBox.SetActive(true);
        }
    }
}
