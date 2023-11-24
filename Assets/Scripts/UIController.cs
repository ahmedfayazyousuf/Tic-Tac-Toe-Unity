using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject[] uI;
    public DatabaseManager manager;
   // public GameController manager2;
    public GameObject FillFields2;
    void Start()
    {
        HideUI();
        uI[0].SetActive(true);
        FillFields2.SetActive(false);
    }
    public void HideUI()
    {
        for(int i=0;i<uI.Length;i++) 
        {
            uI[i].SetActive(false);
        }
    }
    public void ShowUI(int uIIndex)
    {
        
        if (uIIndex != 2)
        {
            HideUI();
            uI[uIIndex].SetActive(true);
        }

         if(uIIndex == 2)
        {
            if (manager.Name.text == "" || manager.Email.text == "")
            {
                FillFields2.SetActive(true);
            }
            else
            {
                HideUI();
                manager.CreateUser();
                uI[uIIndex].SetActive(true);
            }
        }

         if(uIIndex == 3)
        {
            HideUI();
            uI[uIIndex].SetActive(true);
            GameController.instance2.KUKAturn();
        }
    }
}
